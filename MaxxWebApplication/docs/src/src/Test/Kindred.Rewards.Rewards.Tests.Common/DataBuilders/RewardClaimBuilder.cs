using System.Diagnostics;

using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Infrastructure.Data.Exceptions;
using Kindred.Rewards.Core.Infrastructure.Data.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.Rewards;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QueryableExtensions = System.Data.Entity.QueryableExtensions;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public class RewardClaimBuilder : DataBuilderBase
{
    public static RewardClaimDomainModel CreateBonusClaim(RewardType rewardType = RewardType.Freebet, Action<RewardClaimDomainModel> callback = null)
    {
        var model = CreateBase(rewardType);
        model.Terms = RewardBaseDomainModelBuilder.GetTerms(rewardType);
        model.Type = rewardType;

        callback?.Invoke(model);

        return model;
    }

    public static RewardClaimDomainModel CreatePromotionClaim(RewardType rewardType = RewardType.Uniboost, Action<RewardClaimDomainModel> callback = null)
    {
        var model = CreateBase(rewardType);
        model.Terms = RewardBaseDomainModelBuilder.GetTerms(rewardType);
        model.Type = rewardType;

        callback?.Invoke(model);

        return model;
    }

    public static async Task<RewardClaimDomainModel> CreatePromotionClaimInDb(IServiceScopeFactory container, Action<RewardClaimDomainModel> callback)
    {
        var claim = CreatePromotionClaim();

        callback?.Invoke(claim);

        var template = PromotionTemplateBuilder.Create();
        var promo = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Uniboost, p =>
        {
            p.RewardId = claim.RewardId;
        });

        using var scope = container.CreateScope();
        var contextFactory = scope.ServiceProvider.GetService<IDbContextFactory<RewardsDbContext>>();
        await using var context = await contextFactory.CreateDbContextAsync();

        var mapper = scope.ServiceProvider.GetService<IMapper>();

        await AddRewardTemplateDomainModelAsync(context, template, mapper);
        await AddRewardDomainModelAsync(context, promo, mapper);
        await SetPromotionToPromotionTemplateAsync(context, template.TemplateKey, new List<string> { promo.RewardId }, mapper);
        await AddRewardClaimDomainModelAsync(context, claim, mapper);

        return claim;
    }

    private static RewardClaimDomainModel CreateBase(RewardType rewardType)
    {
        var testGuid = Guid.NewGuid();
        var hashedGuid = testGuid.GetHashCode().ToString();
        var bet = ObjectBuilder.CreateBet();

        return new()
        {
            PromotionName = hashedGuid,
            RewardId = testGuid.ToString(),
            InstanceId = hashedGuid,
            CustomerId = hashedGuid,
            CouponRn = hashedGuid,
            BetRn = bet.Rn,
            UsageId = 1,
            IntervalId = 1,
            Status = RewardClaimStatus.Claimed,
            UpdatedOn = DateTime.UtcNow,
            CurrencyCode = TestConstants.DefaultCurrencyCode,
            Bet = bet,
            ClaimLimit = 1,
            Reward = RewardBuilder.CreateReward<RewardDomainModel>(rewardType, null)
        };
    }

    public static async Task<RewardDomainModel> AddRewardDomainModelAsync(RewardsDbContext context, RewardDomainModel domain, IMapper mapper)
    {
        var foundTags = new List<Tag>();
        if (domain.Tags.IsNotNullAndNotEmpty())
        {
            var predicate = SearchPredicateBuilder.BuildSpecificTagsPredicate(domain.Tags);
            var repositoryTags = context.Tags.Where(predicate).ToList();

            domain.Tags = domain.Tags.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            foreach (var tagName in domain.Tags)
            {
                var tag = repositoryTags.Find(tag => tag.Name == tagName);

                if (tag == null)
                {
                    tag = CreateTag(tagName);
                    repositoryTags.Add(tag);
                }

                foundTags.Add(tag);
            }
        }

        var data = mapper.Map<Reward>(domain);

        data.RewardTags = foundTags.Select(t => new RewardTag
        {
            Reward = data,
            Tag = t,
        }).ToList();

        context.Rewards.Add(data);
        OnDataAdded(data, context);
        await context.SaveChangesAsync();

        return domain;
    }

    private static async Task<RewardClaimDomainModel> AddRewardClaimDomainModelAsync(RewardsDbContext context, RewardClaimDomainModel domain, IMapper mapper)
    {
        try
        {
            var data = mapper.Map<RewardClaim>(domain);
            var saved = context.RewardClaims.Add(data);

            Debug.Assert(saved != null, "Failed to create a reward claim");

            OnDataCreated(domain, context.AuditRewards);

            await context.SaveChangesAsync();

            return mapper.Map<RewardClaimDomainModel>(data);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new RewardClaimedConcurrencyException();
        }
    }

    private static void OnDataCreated(RewardClaimDomainModel domain, DbSet<AuditReward> context)
    {
        AuditReward audit = new()
        {
            RewardId = domain.RewardId,
            Activity = nameof(AuditActivity.ClaimCreated),
            CreatedBy = domain.CreatedBy
        };

        context.Add(audit);
    }

    private static async Task<RewardTemplateDomainModel> AddRewardTemplateDomainModelAsync(RewardsDbContext context,
        RewardTemplateDomainModel promotionTemplate, IMapper mapper)
    {
        var data = mapper.Map<RewardTemplate>(promotionTemplate);

        context.RewardTemplates.Add(data);
        await context.SaveChangesAsync();

        return mapper.Map<RewardTemplateDomainModel>(data);
    }

    private static async Task<RewardTemplateDomainModel> SetPromotionToPromotionTemplateAsync(
        RewardsDbContext context, string promotionTemplateKey,
        IEnumerable<string> rewardRns, IMapper mapper)
    {
        var template = QueryableExtensions.Include(context.RewardTemplates, t => t.RewardTemplateReward)
            .FirstOrDefault(t => t.Key == promotionTemplateKey);

        if (template != null)
        {
            template.RewardTemplateReward.Clear();

            if (rewardRns != null)
            {
                var promotions = context.Rewards.Where(p => rewardRns.Contains(p.Id)).ToList();

                template.RewardTemplateReward.AddRange(
                    promotions.Select(
                        p => new RewardTemplateReward
                        {
                            RewardTemplate = template,
                            Reward = p
                        }));

                template.UpdatedOn = DateTime.UtcNow;

                await context.SaveChangesAsync();

                return mapper.Map<RewardTemplateDomainModel>(template);
            }
        }

        throw new InvalidOperationException($"Template with key={promotionTemplateKey} not found");
    }

    private static void OnDataAdded(Reward data, RewardsDbContext context)
    {
        var audit = new AuditReward
        {
            RewardId = data.Id,
            Activity = nameof(AuditActivity.BonusCreated),
            CreatedBy = data.CreatedBy
        };

        context.AuditRewards.Add(audit);
    }

    private static Tag CreateTag(string tagName)
    {
        var dataModel = new Tag
        {
            Name = tagName,
            Comments = DomainConstants.AutoCreatedTagComments
        };

        return dataModel;
    }

}
