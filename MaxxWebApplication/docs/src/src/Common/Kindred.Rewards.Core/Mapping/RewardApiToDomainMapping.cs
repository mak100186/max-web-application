using AutoMapper;

using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Models.Rewards;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Payloads.BetModel;
using Kindred.Rewards.Core.WebApi.Requests;

namespace Kindred.Rewards.Core.Mapping;
public class RewardApiToDomainMapping : Profile
{
    public RewardApiToDomainMapping()
    {
        CreateMap<CreateRewardTemplateRequest, RewardTemplateDomainModel>()
            .ForMember(d => d.Enabled, opt => opt.MapFrom(s => true))
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Rewards, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.CreatedOn, opt => opt.Ignore())
            .ForMember(d => d.UpdatedOn, opt => opt.Ignore());

        CreateMap<PatchRewardRequest, RewardPatchDomainModel>();

        CreateMap<RewardRestrictionApiModel, RewardRestriction>(MemberList.None)
            .ForMember(d => d.StartDateTime, opt => opt.MapFrom(s => s.StartDateTime ?? DateTime.MinValue))
            .ForMember(d => d.ExpiryDateTime, opt => opt.MapFrom(s => s.ExpiryDateTime ?? DateTime.MaxValue))
            .ForMember(d => d.ClaimAllowedPeriod, opt => opt.MapFrom(s => s.ClaimAllowedPeriod))
            .AfterMap((api, domain) =>
            {
                if (string.IsNullOrWhiteSpace(api.ClaimAllowedPeriod))
                {
                    domain.TimezoneId = null;
                    return;
                }

                if (string.IsNullOrWhiteSpace(api.TimezoneId))
                {
                    domain.TimezoneId = TimeZoneInfo.Utc.Id;
                    return;
                }

                domain.TimezoneId = api.TimezoneId;
            });

        CreateMap<GetClaimsByFilterRequest, RewardClaimFilterDomainModel>(MemberList.None)
            .ForMember(d => d.InstanceId, opt => opt.MapFrom(s => s.InstanceId))
            .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
            .ForMember(d => d.RewardRn, opt => opt.MapFrom<RewardRnIdResolver, string>(m => m.RewardRn))
            .ForMember(d => d.RewardId, opt => opt.MapFrom(m => m.RewardRn))
            .ForMember(d => d.RewardName, opt => opt.MapFrom(s => s.RewardName))
            .ForMember(d => d.CouponRn, opt => opt.MapFrom(s => s.CouponRef))
            .ForMember(d => d.BetRn, opt => opt.MapFrom(s => s.BetRef))
            .ForMember(d => d.RewardType, opt => opt.MapFrom(s => s.RewardType))
            .ForMember(d => d.ClaimStatus, opt => opt.MapFrom(s => s.ClaimStatus))
            .ForMember(d => d.BetOutcomeStatus, opt => opt.MapFrom(s => s.BetOutcomeStatus))
            .ForMember(d => d.UpdatedDateFromUtc, o => o.MapFrom((s, d) => s.UpdatedDateFromUtc?.UtcDateTime))
            .ForMember(d => d.UpdatedDateToUtc, o => o.MapFrom((s, d) => s.UpdatedDateToUtc?.UtcDateTime));

        CreateMap<BatchClaimRequest, BatchClaimDomainModel>()
            .ForMember(d => d.CouponRn, opt => opt.MapFrom(s => s.CouponRn))
            .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
            .ForMember(d => d.Claims, opt => opt.MapFrom(s => s.Claims))
            .ForMember(d => d.CurrencyCode, opt => opt.MapFrom(s => s.CurrencyCode));

        CreateMap<ClaimApiModel, BatchClaimItemDomainModel>()
            .ForMember(d => d.RewardId, opt => opt.MapFrom<RewardRnIdResolver, string>(m => m.Rn))
            .ForMember(d => d.Bet, opt => opt.MapFrom(s => s.Bet))
            .ForMember(d => d.Hash, opt => opt.MapFrom(s => s.Hash));

        CreateMap<BetApiModel, BetDomainModel>()
            .ForMember(d => d.Rn, opt => opt.MapFrom(s => s.Rn))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
            .ForMember(d => d.Stages, opt => opt.MapFrom(s => s.Stages))
            .ForMember(d => d.RequestedStake, opt => opt.MapFrom(s => s.RequestedStake))
            .ForMember(d => d.Formula, opt => opt.MapFrom(s => s.Formula));

        CreateMap<CompoundStageApiModel, CompoundStageDomainModel>()
            .ForMember(d => d.Market, opt => opt.MapFrom(s => s.Market))
            .ForMember(d => d.RequestedPrice, opt => opt.MapFrom(s => s.Odds.RequestedPrice))
            .ForMember(d => d.RequestedOutcome, opt => opt.MapFrom(s => s.RequestedSelection.Outcome))
            .ForMember(d => d.ContestType, opt => opt.Ignore())
            .ForMember(d => d.ContestKey, opt => opt.Ignore());

    }
}
