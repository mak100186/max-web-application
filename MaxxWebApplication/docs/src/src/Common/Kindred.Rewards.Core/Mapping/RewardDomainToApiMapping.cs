using AutoMapper;

using Kindred.Infrastructure.Hosting.WebApi.Extensions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Promotions;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Models.Rewards;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Payloads.BetModel;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Responses;

namespace Kindred.Rewards.Core.Mapping;
public class RewardDomainToApiMapping : Profile
{
    public RewardDomainToApiMapping()
    {
        CreateMap<RewardTerms, DomainRestrictionApiModel>()
            .ForMember(d => d.MultiConfig, opt => opt.MapFrom(s
                => new MultiConfigApiModel
                {
                    MinStages = s.GetMinStages(null),
                    MaxStages = s.GetMaxStages(null),
                    FilterFormulae = s.GetAllowedFormulae(null),
                    MaxCombinations = s.GetMaxCombinations(null),
                    MinCombinations = s.GetMinCombinations(null)
                }))
            .ForMember(d => d.FilterOutcomes, opt => opt.MapFrom(s => s.Restrictions.AllowedOutcomes))
            .ForMember(d => d.FilterContestRefs, opt => opt.MapFrom(s => s.Restrictions.AllowedContestRefs))
            .ForMember(d => d.FilterContestStatuses, opt => opt.MapFrom(s => s.Restrictions.AllowedContestStatuses))
            .ForMember(d => d.FilterContestTypes, opt => opt.MapFrom(s => s.Restrictions.AllowedContestTypes))
            .ForMember(d => d.FilterContestCategories, opt => opt.MapFrom(s => s.Restrictions.AllowedContestCategories))
            .ForMember(d => d.OddLimits, opt => opt.MapFrom(s => s.Restrictions.OddLimits));

        CreateMap<OddLimitsConfig, OddLimitsApiModel>();

        CreateMap<RewardSettlement, SettlementApiModel>();

        CreateMap<CompoundStageDomainModel, CompoundStageResponseApiModel>(MemberList.None)
            .ForMember(d => d.Market, opt => opt.MapFrom(s => s.Market))
            .AfterMap((s, d) =>
            {
                d.Odds = new()
                {
                    RequestedPrice = s.RequestedPrice,
                };
                d.RequestedSelection = new()
                {
                    Outcome = s.RequestedOutcome
                };
            });

        CreateMap<BetDomainModel, BetResponseApiModel>(MemberList.None)
            .ForMember(d => d.Rn, opt => opt.MapFrom(s => s.Rn))
            .ForMember(d => d.RequestedStake, opt => opt.MapFrom(s => s.RequestedStake))
            .ForMember(d => d.Formula, opt => opt.MapFrom(s => s.Formula))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
            .AfterMap((s, d, rc) =>
            {
                d.Stages = s.Stages.Select(compoundStageDomainModel => rc.Mapper.Map<CompoundStageResponseApiModel>(compoundStageDomainModel)).ToList();
            });

        CreateMap<RewardClaimDomainModel, ClaimEntitlementResponse>(MemberList.None)
            .ForMember(d => d.Rn, opt => opt.MapFrom(s => s.InstanceId))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
            .ForMember(d => d.Reward, opt => opt.Ignore())
            .ForMember(d => d.CurrencyCode, opt => opt.MapFrom(s => s.CurrencyCode))
            .ForPath(d => d.Bet, opt => opt.MapFrom(s => s.Bet))
            .AfterMap((s, d) =>
            {
                d.Reward = new()
                {
                    Rn = new RewardRn(Guid.Parse(s.RewardId)).ToString(),
                    RewardId = Guid.Parse(s.RewardId).ToString(),
                    Type = s.Type.ToString(),
                    Category = s.Category.ToString(),
                    CustomerId = s.CustomerId,
                    Attributes = new(s.Terms.Attributes),
                    RewardParameters = MapFromRewardTermsToRewardParameters(s.Terms, s.Type)
                };
            });

        CreateMap<RewardClaimDomainModel, RewardEntitlementApiModel>(MemberList.None)
           .ForMember(d => d.Rn, opt => opt.MapFrom<RewardRnResolver, string>(m => m.RewardId))
           .ForMember(d => d.RewardId, opt => opt.MapFrom(m => m.RewardId))
           .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
           .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
           .ForMember(d => d.Hash, opt => opt.MapFrom(s => s.Hash))
           .ForMember(d => d.SettlementTerms, opt => opt.MapFrom(s => s.Terms.SettlementTerms))
           .ForMember(d => d.Attributes, opt => opt.MapFrom(s => s.Terms.Attributes))
           .ForPath(d => d.DomainRestriction, opt => opt.MapFrom(s => s.Terms))
           .AfterMap(
               (s, d) =>
               {
                   d.RewardParameters = MapFromRewardTermsToRewardParameters(s.Terms, s.Type);

                   d.Reporting.Purpose = s.Reward.Purpose;
                   d.Reporting.SubPurpose = s.Reward.SubPurpose;
                   d.Reporting.Name = s.Reward.Name;
                   d.Reporting.IsNameAutoGenerated = s.Reward.IsNameAutoGenerated;
                   d.Reporting.Comments = s.Reward.Comments;
                   d.Reporting.RewardRules = s.Reward.RewardRules;
                   d.Reporting.CustomerFacingName = s.Reward.CustomerFacingName;
                   d.Reporting.CreatedBy = s.Reward.CreatedBy;
                   d.Reporting.CreatedOn = s.Reward.CreatedOn;
                   d.Reporting.UpdatedBy = s.Reward.UpdatedBy;
                   d.Reporting.UpdatedOn = s.Reward.UpdatedOn;
                   d.Reporting.Tags = s.Reward.Tags;

                   d.DateTimeRestrictions.StartDateTime = s.Reward.Terms.Restrictions.StartDateTime;
                   d.DateTimeRestrictions.ExpiryDateTime = s.Reward.Terms.Restrictions.ExpiryDateTime;
                   d.DateTimeRestrictions.ClaimInterval = s.Reward.Terms.Restrictions.ClaimInterval;
                   d.DateTimeRestrictions.ClaimsPerInterval = s.Reward.Terms.Restrictions.ClaimsPerInterval ?? 0;
                   d.DateTimeRestrictions.ClaimAllowedPeriod = s.Reward.Terms.Restrictions.ClaimAllowedPeriod;
                   d.DateTimeRestrictions.RemainingClaimsPerInterval = s.ClaimLimit ?? 0;
                   d.DateTimeRestrictions.TimezoneId = s.Reward.Terms.Restrictions.TimezoneId;

                   d.PlatformRestrictions.CountryCode = s.Reward.CountryCode;
                   d.PlatformRestrictions.Jurisdiction = s.Reward.Jurisdiction;
                   d.PlatformRestrictions.State = s.Reward.State;
                   d.PlatformRestrictions.Brand = s.Reward.Brand;
                   d.PlatformRestrictions.CurrencyCode = s.Reward.CurrencyCode;
               });

        CreateMap<PagedResponse<RewardClaimDomainModel>, GetClaimsResponse>()
            .ForMember(d => d.Offset, opt => opt.MapFrom(s => s.Offset))
            .ForMember(d => d.Limit, opt => opt.MapFrom(s => s.Limit))
            .ForMember(d => d.Offset, opt => opt.MapFrom(s => s.Offset))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));

        CreateMap<RewardClaimDomainModel, ClaimItemApiModel>(MemberList.None)
            .ForMember(d => d.RewardRn, opt => opt.MapFrom<RewardRnResolver, string>(m => m.RewardId))
            .ForMember(d => d.RewardId, opt => opt.MapFrom(m => m.RewardId))
            .ForMember(d => d.InstanceId, opt => opt.MapFrom(s => s.InstanceId))
            .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.RewardType, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.RewardName, opt => opt.MapFrom(s => s.PromotionName))
            .ForMember(d => d.CouponRef, opt => opt.MapFrom(s => s.CouponRn))
            .ForMember(d => d.BetRef, opt => opt.MapFrom(s => s.BetRn))
            .ForMember(d => d.IntervalId, opt => opt.MapFrom(s => s.IntervalId))
            .ForMember(d => d.UsageId, opt => opt.MapFrom(s => s.UsageId))
            .ForMember(d => d.UpdatedOn, opt => opt.MapFrom(s => s.UpdatedOn))
            .ForMember(d => d.RewardPayoutAmount, opt => opt.MapFrom(s => s.RewardPayoutAmount))
            .ForMember(d => d.BetOutcomeStatus, opt => opt.MapFrom(s => s.BetOutcomeStatus))
            .ForMember(d => d.Restrictions, opt => opt.MapFrom(s => s.Terms.Restrictions))
            .ForMember(d => d.SettlementTerms, opt => opt.MapFrom(s => s.Terms.SettlementTerms))
            .ForMember(d => d.Attributes, opt => opt.MapFrom(s => s.Terms.Attributes))
            .ForMember(d => d.CurrencyCode, opt => opt.MapFrom(s => s.CurrencyCode))
            .AfterMap(
                (s, d) =>
                {
                    d.RewardParameters = MapFromRewardTermsToRewardParameters(s.Terms, s.Type);
                });

        CreateMap<RewardRestriction, RewardRestrictionApiModel>(MemberList.None);

        CreateMap<BetDomainModel, BetApiModel>(MemberList.None)
            .ForMember(d => d.Rn, opt => opt.MapFrom(s => s.Rn))
            .ForMember(d => d.RequestedStake, opt => opt.MapFrom(s => s.RequestedStake))
            .ForMember(d => d.Formula, opt => opt.MapFrom(s => s.Formula))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
            .AfterMap((s, d, rc) =>
            {
                d.Stages = s.Stages.Select(compoundStageDomainModel => rc.Mapper.Map<CompoundStageApiModel>(compoundStageDomainModel)).ToList();
            });

        CreateMap<CompoundStageDomainModel, CompoundStageApiModel>(MemberList.None)
            .ForMember(d => d.Market, opt => opt.MapFrom(s => s.Market))
            .AfterMap((s, d) =>
            {
                d.Odds = new()
                {
                    RequestedPrice = s.RequestedPrice
                };
                d.RequestedSelection = new()
                {
                    Outcome = s.RequestedOutcome
                };
            });

        CreateMap<ClaimResultDomainModel, BatchClaimApiModel>()
            .ForMember(d => d.Claim, opt => opt.MapFrom(s => s.Claim));

        CreateMap<BatchClaimResultDomainModel, BatchClaimResponse>()
            .ForMember(d => d.Responses, opt => opt.MapFrom(s => s.ClaimResults));

        CreateMap<RewardTemplateDomainModel, RewardTemplateApiModel>();

        CreateMap<RewardRestriction, DomainRestrictionApiModel>(MemberList.None)
            .ForMember(d => d.FilterContestStatuses, opt => opt.MapFrom(s => s.AllowedContestStatuses))
            .ForMember(d => d.FilterContestTypes, opt => opt.MapFrom(s => s.AllowedContestTypes))
            .ForMember(d => d.FilterContestCategories, opt => opt.MapFrom(s => s.AllowedContestCategories))
            .ForMember(d => d.FilterOutcomes, opt => opt.MapFrom(s => s.AllowedOutcomes))
            .ForMember(d => d.FilterContestRefs, opt => opt.MapFrom(s => s.AllowedContestRefs));

        CreateMap<RewardRestriction, EntitlementRestrictionApiModel>()
            .ForMember(d => d.AllowedContestTypes, opt => opt.MapFrom(s => s.AllowedContestTypes))
            .ForMember(d => d.AllowedOutcomes, opt => opt.MapFrom(s => s.AllowedOutcomes))
            .ForMember(d => d.ClaimLimit, opt => opt.Ignore());

        CreateMap<GetPromotionTemplatesRequest, PromotionTemplateFilterDomainModel>(MemberList.None)
            .ForMember(d => d.IncludeDisabled, opt => opt.MapFrom(s => s.IncludeDisabled))
            .ForMember(d => d.TemplateKey, opt => opt.MapFrom(s => s.TemplateKey));

        CreateMap<GetRewardsRequest, RewardFilterDomainModel>(MemberList.None)
            .ForMember(d => d.IncludeCancelled, opt => opt.MapFrom(s => s.IncludeCancelled))
            .ForMember(d => d.IncludeExpired, opt => opt.MapFrom(s => s.IncludeExpired))
            .ForMember(d => d.UpdatedDateFromUtc, opt => opt.MapFrom((s, d) => s.UpdatedDateFromUtc?.UtcDateTime))
            .ForMember(d => d.UpdatedDateToUtc, opt => opt.MapFrom((s, d) => s.UpdatedDateToUtc?.UtcDateTime))
            .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
            .ForMember(d => d.SortBy, opt => opt.MapFrom(s => s.SortBy))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.RewardType, opt => opt.MapFrom(s => s.RewardType))
            .ForMember(d => d.Jurisdiction, opt => opt.MapFrom(s => s.Jurisdiction.Trim().ToUpperInvariant()))
            .ForMember(d => d.Country, opt => opt.MapFrom(s => s.Country.Trim().ToUpperInvariant()))
            .ForMember(d => d.Brand, opt => opt.MapFrom(s => s.Brand.Trim().ToUpperInvariant()))
            .ForMember(d => d.State, opt => opt.MapFrom(s => s.State.Trim().ToUpperInvariant()));

        CreateMap<RewardReloadApiModel, RewardReloadConfig>(MemberList.None)
            .ForMember(d => d.MaxReload, opt => opt.MapFrom(s => s.MaxReload))
            .ForMember(d => d.StopOnMinimumWinBets, opt => opt.MapFrom(s => s.StopOnMinimumWinBets));

        CreateMap<SettlementApiModel, RewardSettlement>();


        CreateMap<RewardReloadConfig, RewardReloadApiModel>(MemberList.None)
            .ForMember(d => d.MaxReload, opt => opt.MapFrom(s => s.MaxReload))
            .ForMember(d => d.StopOnMinimumWinBets, opt => opt.MapFrom(s => s.StopOnMinimumWinBets));

        CreateMap<RewardTemplateDomainModel, RewardTemplateResponse>();



        CreateMap<RewardDomainModel, GetRewardResponse>(MemberList.None)
            .ForMember(d => d.Rn, opt => opt.MapFrom<RewardRnResolver, string>(m => m.RewardId))
            .ForMember(d => d.RewardId, opt => opt.MapFrom(m => m.RewardId))
            .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
            .ForMember(d => d.SettlementTerms, opt => opt.MapFrom(s => s.Terms.SettlementTerms))
            .ForMember(d => d.Attributes, opt => opt.MapFrom(s => s.Terms.Attributes))
            .ForPath(d => d.DomainRestriction, opt => opt.MapFrom(s => s.Terms))

            .AfterMap(
                (s, d) =>
                {
                    d.RewardParameters = MapFromRewardTermsToRewardParameters(s.Terms, s.Type);

                    d.Status = s.GetCurrentStatus().ToString();
                    d.PromotionRns = s.Templates?.Select(x => x.TemplateKey).ToList();
                    d.Reporting.Purpose = s.Purpose;
                    d.Reporting.Purpose = s.Purpose;
                    d.Reporting.SubPurpose = s.SubPurpose;
                    d.Reporting.Name = s.Name;
                    d.Reporting.IsCancelled = s.IsCancelled;
                    d.Reporting.CancellationReason = s.CancellationReason;
                    d.Reporting.Comments = s.Comments;
                    d.Reporting.RewardRules = s.RewardRules;
                    d.Reporting.IsNameAutoGenerated = s.IsNameAutoGenerated;
                    d.Reporting.CustomerFacingName = s.CustomerFacingName;
                    d.Reporting.CreatedBy = s.CreatedBy;
                    d.Reporting.CreatedOn = s.CreatedOn;
                    d.Reporting.UpdatedBy = s.UpdatedBy;
                    d.Reporting.UpdatedOn = s.UpdatedOn;
                    d.Reporting.Tags = s.Tags;

                    d.DateTimeRestrictions.StartDateTime = s.Terms.Restrictions.StartDateTime;
                    d.DateTimeRestrictions.ExpiryDateTime = s.Terms.Restrictions.ExpiryDateTime;
                    d.DateTimeRestrictions.ClaimInterval = s.Terms.Restrictions.ClaimInterval;
                    d.DateTimeRestrictions.ClaimsPerInterval = s.Terms.Restrictions.ClaimsPerInterval ?? 0;
                    d.DateTimeRestrictions.ClaimAllowedPeriod = s.Terms.Restrictions.ClaimAllowedPeriod;
                    d.DateTimeRestrictions.RemainingClaimsPerInterval = s.Terms.Restrictions.ClaimsPerInterval ?? 0;
                    d.DateTimeRestrictions.TimezoneId = s.Terms.Restrictions.TimezoneId;

                    d.PlatformRestrictions.CountryCode = s.CountryCode;
                    d.PlatformRestrictions.Jurisdiction = s.Jurisdiction;
                    d.PlatformRestrictions.State = s.State;
                    d.PlatformRestrictions.Brand = s.Brand;
                    d.PlatformRestrictions.CurrencyCode = s.CurrencyCode;
                });

        CreateMap<RewardDomainModel, RewardApiModel>(MemberList.None)
            .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.Comments, opt => opt.MapFrom(s => s.Comments))
            .ForMember(d => d.RewardRules, opt => opt.MapFrom(s => s.RewardRules))
            .ForMember(d => d.IsNameAutoGenerated, opt => opt.MapFrom(s => s.IsNameAutoGenerated))
            .ForMember(d => d.RewardCategory, opt => opt.MapFrom(s => s.Category))
            .ForMember(d => d.RewardType, opt => opt.MapFrom(s => s.Type))
            .ForMember(d => d.RewardRn, opt => opt.MapFrom<RewardRnResolver, string>(m => m.RewardId))
            .ForMember(d => d.RewardId, opt => opt.MapFrom(m => m.RewardId))
            .ForMember(d => d.IsCancelled, opt => opt.MapFrom(s => s.IsCancelled))
            .ForMember(d => d.CancellationReason, opt => opt.MapFrom(s => s.CancellationReason))
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.Tags))
            .ForMember(d => d.CountryCode, opt => opt.MapFrom(s => s.CountryCode))
            .ForMember(d => d.Jurisdiction, opt => opt.MapFrom(s => s.Jurisdiction))
            .ForMember(d => d.State, opt => opt.MapFrom(d => d.State))
            .ForMember(d => d.Brand, opt => opt.MapFrom(s => s.Brand))
            .ForMember(d => d.Purpose, opt => opt.MapFrom(s => s.Purpose))
            .ForMember(d => d.SubPurpose, opt => opt.MapFrom(s => s.SubPurpose))
            .ForMember(d => d.CustomerFacingName, o => o.MapFrom(s => s.CustomerFacingName))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.Templates, opt => opt.MapFrom(s => s.Templates))
            .ForPath(d => d.Restrictions, opt => opt.MapFrom(s => s.Terms.Restrictions))
            .ForPath(d => d.SettlementTerms, opt => opt.MapFrom(s => s.Terms.SettlementTerms))
            .ForPath(d => d.Attributes, opt => opt.MapFrom(s => s.Terms.Attributes))
            .ForPath(d => d.DomainRestriction, opt => opt.MapFrom(s => s.Terms))

            .AfterMap(
                (s, d) =>
                {
                    d.RewardParameters = MapFromRewardTermsToRewardParameters(s.Terms, s.Type);
                });

        CreateMap<PagedResponse<RewardTemplateDomainModel>, GetPromotionTemplatesResponse>()
            .ForMember(d => d.Offset, opt => opt.MapFrom(s => s.Offset))
            .ForMember(d => d.Limit, opt => opt.MapFrom(s => s.Limit))
            .ForMember(d => d.Offset, opt => opt.MapFrom(s => s.Offset))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));

        CreateMap<PagedResponse<RewardDomainModel>, GetRewardsResponse>()
            .ForMember(d => d.Limit, opt => opt.MapFrom(s => s.Limit))
            .ForMember(d => d.Offset, opt => opt.MapFrom(s => s.Offset))
            .ForMember(d => d.ItemCount, opt => opt.MapFrom(s => s.ItemCount))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));



        CreateMap<RewardInfoDomainModel, GetAllowedMarketTypesResponse>()
        .ForMember(d => d.OptionalParameterKeys, opt => opt.MapFrom(s => s.OptionalParameterKeys))
        .ForMember(d => d.RequiredParameterKeys, opt => opt.MapFrom(s => s.RequiredParameterKeys));
    }
    private static RewardParameterApiModelBase MapFromRewardTermsToRewardParameters(
        RewardTerms terms, RewardType rewardType)
    {
        return RewardParameterMappingHelper.MapFromRewardTermsToRewardParameters(terms, rewardType);
    }
}
