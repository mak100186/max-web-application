using AutoMapper;

using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.Rewards;

using Newtonsoft.Json;

namespace Kindred.Rewards.Core.Mapping;

public class RewardDomainToDataMapping : Profile
{
    public RewardDomainToDataMapping()
    {
        CreateMap<RewardDomainModel, Reward>(MemberList.None)
            .MapRewardBase()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
            .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CustomerId))
            .ForMember(d => d.RewardTemplateReward, o => o.Ignore())
            .ForMember(d => d.Audits, o => o.Ignore())
            .ForMember(d => d.RewardTags, o => o.Ignore())
            .ForMember(d => d.IsSystemGenerated, o => o.MapFrom(s => s.IsSystemGenerated))
            .ForMember(d => d.IsLocked, o => o.MapFrom(s => s.IsLocked))
            .ForMember(d => d.CustomerRewards, opt => opt.Ignore());

        CreateMap<RewardTemplateDomainModel, RewardTemplate>()
            .MapEditableBase()
            .ForMember(d => d.Key, o => o.MapFrom(s => s.TemplateKey))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
            .ForMember(d => d.RewardTemplateReward, o => o.Ignore()) //will be populated in AfterMap
            .ForMember(d => d.Enabled, o => o.MapFrom(s => s.Enabled))
            .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .AfterMap(
                (pDomMod, pDataMod, rc) => pDataMod.RewardTemplateReward =
                    pDomMod.Rewards == null
                        ? new()
                        : pDomMod.Rewards.Select(
                            p => new RewardTemplateReward
                            {
                                Reward = rc.Mapper.Map<Reward>(p),
                                RewardTemplate = pDataMod
                            }).ToList());

        CreateMap<RewardClaimDomainModel, RewardClaim>(MemberList.None)
            .MapEditableBase()
            .ForMember(d => d.CouponRn, o => o.MapFrom(s => s.CouponRn))
            .ForMember(d => d.BetRn, o => o.MapFrom(s => s.BetRn))
            .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CustomerId))
            .ForMember(d => d.IntervalId, o => o.MapFrom(s => s.IntervalId))
            .ForMember(d => d.RewardId, o => o.MapFrom(s => s.RewardId))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.TermsJson, o => o.MapFrom((s, d) => JsonConvert.SerializeObject(s.Terms)))
            .ForMember(d => d.UsageId, o => o.MapFrom(s => s.UsageId))
            .ForMember(d => d.RewardPayoutAmount, o => o.MapFrom(s => s.RewardPayoutAmount))
            .ForMember(d => d.RewardName, o => o.MapFrom(s => s.PromotionName))
            .ForMember(d => d.RewardType, o => o.MapFrom(s => s.Type))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.BetOutcomeStatus, o => o.MapFrom(s => s.BetOutcomeStatus))
            .ForMember(d => d.BetJson, o => o.MapFrom((s, d) => JsonConvert.SerializeObject(s.Bet)));

        CreateMap<Template, RewardTemplate>()
            .MapEditableBase()
            .ForMember(d => d.Key, o => o.MapFrom(s => s.Title))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => DateTime.UtcNow))
            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => DateTime.UtcNow))
            .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => string.Empty))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => string.Empty))
            .ForMember(d => d.Enabled, o => o.MapFrom(s => true))
            .ForMember(d => d.RewardTemplateReward, o => o.MapFrom(s => new List<RewardTemplateReward>()));

        CreateMap<RewardClaimPayoffMetadataDomainModel, RewardClaimPayoffMetadata>();
        CreateMap<RewardClaimOddsMetadataDomainModel, RewardClaimOddsMetadata>();

        CreateMap<CombinationRewardPayoffDomainModel, CombinationRewardPayoff>()
            .ForMember(d => d.RewardClaim, o => o.Ignore())
            .ForMember(d => d.UpdatedOn, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore());
    }
}
