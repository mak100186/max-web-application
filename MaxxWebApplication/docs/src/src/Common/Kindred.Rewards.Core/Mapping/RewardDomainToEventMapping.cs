using AutoMapper;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Messages;
using Kindred.Rewards.Core.Models.Messages.Reward;
using Kindred.Rewards.Core.Models.Messages.Reward.Parameters;
using Kindred.Rewards.Core.Models.Messages.RewardTemplate;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Models.Rewards;

using Newtonsoft.Json;

using RewardRestriction = Kindred.Rewards.Core.Models.RewardConfiguration.RewardRestriction;
using RewardSettlement = Kindred.Rewards.Core.Models.RewardConfiguration.RewardSettlement;
using RewardTerms = Kindred.Rewards.Core.Models.RewardConfiguration.RewardTerms;

namespace Kindred.Rewards.Core.Mapping;

public class RewardDomainToEventMapping : Profile
{
    public RewardDomainToEventMapping()
    {
        CreateMap<RewardDomainModel, Reward>(MemberList.None)
            .ForMember(e => e.RewardRn, opt => opt.MapFrom<RewardRnResolver, string>(m => m.RewardId))
            .ForMember(e => e.RewardId, opt => opt.MapFrom(m => m.RewardId))
            .ForMember(e => e.CustomerId, opt => opt.MapFrom(d => d.CustomerId))
            .ForMember(e => e.RewardType, opt => opt.MapFrom(d => d.Type))
            .ForMember(e => e.Name, opt => opt.MapFrom(d => d.Name))
            .ForMember(e => e.Comments, opt => opt.MapFrom(d => d.Comments))
            .ForMember(e => e.RewardRules, opt => opt.MapFrom(d => d.RewardRules))
            .ForMember(e => e.Restrictions, opt => opt.MapFrom(d => d.Terms.Restrictions))
            .ForMember(e => e.Attributes, opt => opt.MapFrom(d => d.Terms.Attributes))
            .ForMember(e => e.Tags, opt => opt.MapFrom(d => d.Tags))
            .ForMember(e => e.CountryCode, opt => opt.MapFrom(d => d.CountryCode))
            .ForMember(e => e.Jurisdiction, opt => opt.MapFrom(d => d.Jurisdiction))
            .ForMember(e => e.State, opt => opt.MapFrom(d => d.State))
            .ForMember(e => e.Brand, opt => opt.MapFrom(d => d.Brand))
            .ForMember(e => e.Purpose, opt => opt.MapFrom(d => d.Purpose))
            .ForMember(e => e.SubPurpose, opt => opt.MapFrom(d => d.SubPurpose))
            .ForMember(e => e.CustomerFacingName, opt => opt.MapFrom(d => d.CustomerFacingName))
            .ForMember(e => e.CurrencyCode, opt => opt.MapFrom(d => d.CurrencyCode))
            .ForMember(e => e.CreatedBy, opt => opt.MapFrom(d => d.CreatedBy))
            .ForMember(e => e.CreatedOn, opt => opt.MapFrom(d => d.CreatedOn))
            .ForPath(e => e.DomainRestriction.OddLimits, opt => opt.MapFrom(d => d.Terms.Restrictions.OddLimits))
            .AfterMap(
                (domain, evnt) =>
                {
                    if (domain.Terms.RewardParameters.ContainsKey(RewardParameterKey.MinStages))
                    {
                        evnt.DomainRestriction.MultiConfig.MinStages = int.Parse(domain.Terms.RewardParameters[RewardParameterKey.MinStages]);
                    }

                    if (domain.Terms.RewardParameters.ContainsKey(RewardParameterKey.MaxStages))
                    {
                        evnt.DomainRestriction.MultiConfig.MaxStages = int.Parse(domain.Terms.RewardParameters[RewardParameterKey.MaxStages]);
                    }

                    if (domain.Terms.RewardParameters.ContainsKey(RewardParameterKey.MinCombinations))
                    {
                        evnt.DomainRestriction.MultiConfig.MinCombinations = int.Parse(domain.Terms.RewardParameters[RewardParameterKey.MinCombinations]);
                    }

                    if (domain.Terms.RewardParameters.ContainsKey(RewardParameterKey.MaxCombinations))
                    {
                        evnt.DomainRestriction.MultiConfig.MaxCombinations = int.Parse(domain.Terms.RewardParameters[RewardParameterKey.MaxCombinations]);
                    }

                    if (domain.Terms.RewardParameters.ContainsKey(RewardParameterKey.AllowedFormulae))
                    {
                        evnt.DomainRestriction.MultiConfig.FilterFormulae = domain.Terms.RewardParameters[RewardParameterKey.AllowedFormulae].Split(",");
                    }

                    evnt.DomainRestriction.FilterContestRefs = domain.Terms.Restrictions.AllowedContestRefs;
                    evnt.DomainRestriction.FilterContestTypes = domain.Terms.Restrictions.AllowedContestTypes;
                    evnt.DomainRestriction.FilterOutcomes = domain.Terms.Restrictions.AllowedOutcomes;
                    evnt.RewardParameters = MapFromDictionaryToRewardParameters(domain.Terms, domain.Type);
                    evnt.RewardParameters.Type = domain.Type.ToString();

                });

        CreateMap<RewardDomainModel, RewardUpdated>(MemberList.None)
            .IncludeBase<RewardDomainModel, Reward>()
            .ForMember(e => e.UpdatedBy, opt => opt.MapFrom(m => m.UpdatedBy))
            .ForMember(e => e.UpdatedOn, opt => opt.MapFrom(m => m.UpdatedOn))
            .ForMember(e => e.IsCancelled, opt => opt.MapFrom(m => m.IsCancelled))
            .ForMember(e => e.CancellationReason, opt => opt.MapFrom(m => m.CancellationReason));

        CreateMap<RewardDomainModel, RewardCreated>(MemberList.None)
            .IncludeBase<RewardDomainModel, Reward>();

        CreateMap<RewardTemplateDomainModel, RewardTemplate>()
            .ForMember(d => d.Comments, opt => opt.MapFrom(m => m.Comments))
            .ForMember(d => d.CreatedOn, opt => opt.MapFrom(m => m.CreatedOn))
            .ForMember(d => d.Enabled, opt => opt.MapFrom(m => m.Enabled))
            .ForMember(d => d.TemplateKey, opt => opt.MapFrom(m => m.TemplateKey))
            .ForMember(d => d.Title, opt => opt.MapFrom(m => m.Title))
            .ForMember(d => d.Rewards, opt => opt.MapFrom(m => m.Rewards));

        CreateMap<RewardTemplateDomainModel, RewardTemplateCreated>()
            .IncludeBase<RewardTemplateDomainModel, RewardTemplate>();

        CreateMap<RewardTemplateDomainModel, RewardTemplateUpdated>()
            .IncludeBase<RewardTemplateDomainModel, RewardTemplate>();

        CreateMap<RewardClaimDomainModel, RewardClaim>()
            .ForMember(d => d.RewardRn, opt => opt.MapFrom<RewardRnResolver, string>(m => m.RewardId))
            .ForMember(d => d.RewardId, opt => opt.MapFrom(m => m.RewardId))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
            .ForMember(d => d.InstanceId, opt => opt.MapFrom(s => s.InstanceId))
            .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
            .ForMember(d => d.CouponRef, opt => opt.MapFrom(s => s.CouponRn))
            .ForMember(d => d.BetRef, opt => opt.MapFrom(s => s.BetRn))
            .ForMember(d => d.RewardPayoutAmount, opt => opt.MapFrom(s => s.RewardPayoutAmount))
            .ForMember(d => d.RewardName, opt => opt.MapFrom(s => s.PromotionName))
            .ForMember(d => d.Terms, opt => opt.MapFrom(s => s.Terms));

        CreateMap<RewardClaimDomainModel, RewardClaimUnsettled>()
            .IncludeBase<RewardClaimDomainModel, RewardClaim>();

        CreateMap<RewardClaimDomainModel, RewardClaimSettled>()
            .IncludeBase<RewardClaimDomainModel, RewardClaim>();

        CreateMap<RewardTerms, Models.Messages.RewardTerms>()
            .ForMember(d => d.SettlementTerms, opt => opt.MapFrom(s => s.SettlementTerms))
            .ForMember(d => d.RewardParameters, opt => opt.MapFrom(s => s.RewardParameters));

        CreateMap<RewardSettlement, Models.Messages.RewardSettlement>();

        CreateMap<RewardRestriction, Models.Messages.Reward.RewardRestriction>(MemberList.None);

        CreateMap<OddLimitsConfig, OddLimits>(MemberList.None);
    }
    private RewardParametersBase MapFromDictionaryToRewardParameters(
        RewardTerms terms,
        RewardType rewardType)
    {
        var rewardParameters = terms.RewardParameters;
        switch (rewardType)
        {
            case RewardType.Freebet:
                var freebetParameters = new FreeBetParameters();

                if (rewardParameters.ContainsKey(RewardParameterKey.Amount) && !string.IsNullOrWhiteSpace(rewardParameters[RewardParameterKey.Amount]))
                {
                    freebetParameters.Amount = decimal.Parse(rewardParameters[RewardParameterKey.Amount]);
                }

                if (rewardParameters.ContainsKey(RewardParameterKey.MaxExtraWinnings) && !string.IsNullOrWhiteSpace(rewardParameters[RewardParameterKey.MaxExtraWinnings]))
                {
                    freebetParameters.MaxExtraWinnings = decimal.Parse(rewardParameters[RewardParameterKey.MaxExtraWinnings]);
                }

                return freebetParameters;

            case RewardType.UniboostReload:
                var uniboostReloadParameters = new UniBoostReloadParameters();
                SetUniBoostBaseParameters(uniboostReloadParameters, rewardParameters);

                uniboostReloadParameters.Reload = new()
                {
                    MaxReload = terms.Restrictions.Reload?.MaxReload,
                    StopOnMinimumWinBets = terms.Restrictions.Reload.StopOnMinimumWinBets
                };

                return uniboostReloadParameters;


            case RewardType.Uniboost:
                var uniboostParameters = new UniBoostParameters();

                SetUniBoostBaseParameters(uniboostParameters, rewardParameters);

                return uniboostParameters;

            case RewardType.Profitboost:
                var profitboostParameters = new ProfitBoostParameters();

                if (rewardParameters.ContainsKey(RewardParameterKey.MaxStakeAmount) && !string.IsNullOrWhiteSpace(rewardParameters[RewardParameterKey.MaxStakeAmount]))
                {
                    profitboostParameters.MaxStakeAmount = decimal.Parse(rewardParameters[RewardParameterKey.MaxStakeAmount]);
                }

                if (rewardParameters.ContainsKey(RewardParameterKey.LegTable) && !string.IsNullOrWhiteSpace(rewardParameters[RewardParameterKey.LegTable]))
                {
                    profitboostParameters.LegTable = JsonConvert.DeserializeObject<Dictionary<int, decimal>>(rewardParameters[RewardParameterKey.LegTable]);
                }

                if (rewardParameters.ContainsKey(RewardParameterKey.MaxExtraWinnings) && !string.IsNullOrWhiteSpace(rewardParameters[RewardParameterKey.MaxExtraWinnings]))
                {
                    profitboostParameters.MaxExtraWinnings = decimal.Parse(rewardParameters[RewardParameterKey.MaxExtraWinnings]);
                }

                return profitboostParameters;

            default:
                return new();
        }
    }

    private void SetUniBoostBaseParameters(UniBoostParameters uniBoostBaseParameters, IDictionary<string, string> rewardParameters)
    {
        if (rewardParameters.ContainsKey(RewardParameterKey.MaxStakeAmount) && !string.IsNullOrWhiteSpace(rewardParameters[RewardParameterKey.MaxStakeAmount]))
        {
            uniBoostBaseParameters.MaxStakeAmount = decimal.Parse(rewardParameters[RewardParameterKey.MaxStakeAmount]);
        }

        if (rewardParameters.ContainsKey(RewardParameterKey.OddsLadderOffset) && !string.IsNullOrWhiteSpace(rewardParameters[RewardParameterKey.OddsLadderOffset]))
        {
            uniBoostBaseParameters.OddsLadderOffset = int.Parse(rewardParameters[RewardParameterKey.OddsLadderOffset]);
        }

        if (rewardParameters.ContainsKey(RewardParameterKey.MaxExtraWinnings) && !string.IsNullOrWhiteSpace(rewardParameters[RewardParameterKey.MaxExtraWinnings]))
        {
            uniBoostBaseParameters.MaxExtraWinnings = decimal.Parse(rewardParameters[RewardParameterKey.MaxExtraWinnings]);
        }
    }
}
