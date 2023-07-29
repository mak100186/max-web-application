using AutoMapper;

using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Plugin.UniBoost.Models;

namespace Kindred.Rewards.Plugin.UniBoost.Mappings;

public class UniBoostDomainToApiMapping : Profile
{
    public UniBoostDomainToApiMapping()
    {
        CreateMap<RewardDomainModel, UniBoostRewardResponse>(MemberList.None)
            .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.Comments, opt => opt.MapFrom(s => s.Comments))
            .ForMember(d => d.RewardRules, opt => opt.MapFrom(s => s.RewardRules))
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
            .ForMember(d => d.CreatedBy, opt => opt.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.CreatedOn, opt => opt.MapFrom(s => s.CreatedOn))
            .ForMember(d => d.UpdatedBy, opt => opt.MapFrom(s => s.UpdatedBy))
            .ForMember(d => d.UpdatedOn, opt => opt.MapFrom(s => s.UpdatedOn))
            .ForPath(d => d.Restrictions, opt => opt.MapFrom(s => s.Terms.Restrictions))
            .ForPath(d => d.SettlementTerms, opt => opt.MapFrom(s => s.Terms.SettlementTerms))
            .ForPath(d => d.Attributes, opt => opt.MapFrom(s => s.Terms.Attributes))
            .ForPath(d => d.DomainRestriction, opt => opt.MapFrom(s => s.Terms))

            .AfterMap(
                (s, d) =>
                {
                    d.RewardParameters = (UniBoostParametersApiModel)Extensions.Extensions.MapFromRewardTermsToRewardParameters(s.Terms, s.Type);
                });
    }
}
