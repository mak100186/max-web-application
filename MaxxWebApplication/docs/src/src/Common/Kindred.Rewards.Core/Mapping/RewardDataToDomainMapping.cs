using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Models.Rewards;

using Newtonsoft.Json;

namespace Kindred.Rewards.Core.Mapping;

public class RewardDataToDomainMapping : Profile
{
    private static readonly Dictionary<string, RewardStatus> s_auditActivityToExternalStatus = new()
    {
        { nameof(AuditActivity.BonusCreated), RewardStatus.Active },
        { nameof(AuditActivity.BonusUpdated), RewardStatus.Active },
        { nameof(AuditActivity.ClaimCreated), RewardStatus.Claimed },
        { nameof(AuditActivity.ClaimRevoked), RewardStatus.Active },
        { nameof(AuditActivity.ClaimSettled), RewardStatus.Settled },
        { nameof(AuditActivity.ClaimUnsettled), RewardStatus.Claimed },
        { nameof(AuditActivity.BonusCancelled), RewardStatus.Cancelled }
    };

    public RewardDataToDomainMapping()
    {
        CreateMap<Reward, RewardDomainModel>()
            .MapToRewardBase()
            .ForSourceMember(d => d.RewardTags, o => o.DoNotValidate())
            .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CustomerId))
            .ForMember(d => d.RewardId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn.AsUtcDateTime()))
            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn.AsUtcDateTime()))
            .ForMember(d => d.StatusUpdates, opt => opt.MapFrom((s, d) => s.Audits?.OrderBy(a => a.CreatedOn.AsUtcDateTime())))
            .ForMember(d => d.Templates, opt => opt.MapFrom(s => s.RewardTemplateReward.Select(ptp => ptp.RewardTemplate)))
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.RewardTags.Select(bt => bt.Tag.Name)))
            .ForMember(d => d.CustomerFacingName, o => o.MapFrom(s => s.CustomerFacingName));

        CreateMap<RewardTemplate, RewardTemplateDomainModel>()
            .MapFromEditableBase()
            .ForMember(d => d.TemplateKey, o => o.MapFrom(s => s.Key))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
            .ForMember(d => d.Rewards, o => o.MapFrom(s => s.RewardTemplateReward == null ? new List<Reward>() : s.RewardTemplateReward.Select(ptp => ptp.Reward)))
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn.AsUtcDateTime()))
            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn.AsUtcDateTime()))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
            .ForMember(d => d.Enabled, o => o.MapFrom(s => s.Enabled));

        CreateMap<RewardClaim, RewardClaimDomainModel>(MemberList.None)
            .MapFromEditableBase()
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn.AsUtcDateTime()))
            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn.AsUtcDateTime()))
            .ForMember(d => d.CouponRn, o => o.MapFrom(s => s.CouponRn))
            .ForMember(d => d.BetRn, o => o.MapFrom(s => s.BetRn))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CustomerId))
            .ForMember(d => d.IntervalId, o => o.MapFrom(s => s.IntervalId))
            .ForMember(d => d.RewardId, o => o.MapFrom(s => s.RewardId))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
            .ForMember(d => d.UsageId, o => o.MapFrom(s => s.UsageId))
            .ForMember(d => d.InstanceId, o => o.MapFrom(s => s.InstanceId))
            .ForMember(d => d.RewardPayoutAmount, o => o.MapFrom(s => s.RewardPayoutAmount))
            .ForMember(d => d.Type, o => o.MapFrom(s => s.RewardType))
            .ForMember(d => d.ClaimLimit, o => o.Ignore())
            .ForMember(d => d.PromotionName, o => o.Ignore())
            .ForMember(d => d.Terms, o => o.MapFrom((s, d) => JsonConvert.DeserializeObject<RewardTerms>(s.TermsJson)))
            .ForMember(d => d.Bet, o => o.MapFrom((s, d) => JsonConvert.DeserializeObject<BetDomainModel>(s.BetJson)))
            .ForMember(d => d.BetOutcomeStatus, o => o.MapFrom(s => s.BetOutcomeStatus))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.NextInterval, o => o.Ignore())
            .ForMember(d => d.PromotionName, o => o.MapFrom(s => s.RewardName))
            .AfterMap(
                (src, dest) =>
                    {
                        dest.Terms.Restrictions.StartDateTime = DateTime.SpecifyKind(dest.Terms.Restrictions.StartDateTime, DateTimeKind.Utc);
                        dest.Terms.Restrictions.ExpiryDateTime = DateTime.SpecifyKind(dest.Terms.Restrictions.ExpiryDateTime, DateTimeKind.Utc);
                    });
        CreateMap<AuditReward, RewardStatusDomainModel>()
            .ForSourceMember(d => d.Id, o => o.DoNotValidate())
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.CreatedOn.AsUtcDateTime()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s_auditActivityToExternalStatus[s.Activity]));

        CreateMap<RewardClaimPayoffMetadata, RewardClaimPayoffMetadataDomainModel>();
        CreateMap<RewardClaimOddsMetadata, RewardClaimOddsMetadataDomainModel>();

        CreateMap<CombinationRewardPayoff, CombinationRewardPayoffDomainModel>();
    }
}
