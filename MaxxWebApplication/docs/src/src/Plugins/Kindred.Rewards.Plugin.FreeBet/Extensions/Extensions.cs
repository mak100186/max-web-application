using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.WebApi.Payloads;

using Newtonsoft.Json;

namespace Kindred.Rewards.Plugin.FreeBet.Extensions;

public static class Extensions
{
    public static IMappingExpression<TSrc, TDest> MapToRewardBase<TSrc, TDest>(
        this IMappingExpression<TSrc, TDest> m)
        where TSrc : Reward
        where TDest : RewardDomainModel
    {
        return m
            .ForSourceMember(d => d.CreatedOn, o => o.DoNotValidate())
            .ForSourceMember(d => d.UpdatedOn, o => o.DoNotValidate())

            .ForMember(d => d.CancellationReason, o => o.MapFrom(s => s.CancellationReason))
            .ForMember(d => d.IsCancelled, o => o.MapFrom(s => s.IsCancelled))
            .ForMember(d => d.RewardId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Category, o => o.MapFrom(s => Enum.Parse<RewardType>(s.RewardType).ToRewardCategory()))
            .ForMember(d => d.Type, o => o.MapFrom(s => s.RewardType))
            .ForMember(d => d.SourceInstanceId, o => o.MapFrom(s => s.SourceInstanceId))
            .ForMember(d => d.CountryCode, o => o.MapFrom(s => s.CountryCode))
            .ForMember(d => d.Jurisdiction, o => o.MapFrom(s => s.Jurisdiction))
            .ForMember(d => d.State, o => o.MapFrom(s => s.State))
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand))
            .ForMember(d => d.Purpose, o => o.MapFrom(s => s.Purpose))
            .ForMember(d => d.SubPurpose, o => o.MapFrom(s => s.SubPurpose))
            .ForMember(d => d.CustomerFacingName, o => o.MapFrom(s => s.CustomerFacingName))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
            .ForMember(d => d.RewardRules, o => o.MapFrom(s => s.RewardRules))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn.AsUtcDateTime()))
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn.AsUtcDateTime()))
            .ForMember(d => d.Terms, o => o.MapFrom((s, d) => JsonConvert.DeserializeObject<RewardTerms>(s.TermsJson)))
            .AfterMap(
                (src, dest) =>
                    {
                        dest.Terms.Restrictions.StartDateTime = DateTime.SpecifyKind(dest.Terms.Restrictions.StartDateTime, DateTimeKind.Utc);
                        dest.Terms.Restrictions.ExpiryDateTime = DateTime.SpecifyKind(dest.Terms.Restrictions.ExpiryDateTime, DateTimeKind.Utc);
                        if (!string.IsNullOrWhiteSpace(src.ContestStatus))
                        {
                            dest.Terms.Restrictions.AllowedContestStatuses = Enum.Parse<ContestStatus>(src.ContestStatus);
                        }
                    });
    }

    public static IMappingExpression<TSrc, TDest> MapRewardBase<TSrc, TDest>(
        this IMappingExpression<TSrc, TDest> m)
        where TSrc : RewardDomainModel
        where TDest : Reward
    {
        return m
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.UpdatedOn, o => o.Ignore())

            .ForMember(d => d.StartDateTime, o => o.MapFrom((s, d) => s.Terms.Restrictions.StartDateTime.ApplyUtc()))
            .ForMember(d => d.ExpiryDateTime, o => o.MapFrom((s, d) => s.Terms.Restrictions.ExpiryDateTime.ApplyUtc()))

            .ForMember(d => d.ContestStatus, o => o.MapFrom((s, d) => s.Terms.Restrictions.AllowedContestStatuses))

            .ForMember(d => d.IsCancelled, o => o.MapFrom(s => s.IsCancelled))
            .ForMember(d => d.CancellationReason, o => o.MapFrom(s => s.CancellationReason))
            .ForMember(d => d.SourceInstanceId, o => o.MapFrom(s => s.SourceInstanceId))
            .ForMember(d => d.Id, o => o.MapFrom(s => s.RewardId))
            .ForMember(d => d.RewardType, o => o.MapFrom(s => s.Type))
            .ForMember(d => d.TermsJson, o => o.MapFrom((s, d) => JsonConvert.SerializeObject(s.Terms)))
            .ForMember(d => d.CountryCode, o => o.MapFrom(s => s.CountryCode))
            .ForMember(d => d.Jurisdiction, o => o.MapFrom(s => s.Jurisdiction))
            .ForMember(d => d.State, o => o.MapFrom(s => s.State))
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand))
            .ForMember(d => d.Purpose, o => o.MapFrom(s => s.Purpose))
            .ForMember(d => d.SubPurpose, o => o.MapFrom(s => s.SubPurpose))
            .ForMember(d => d.CustomerFacingName, o => o.MapFrom(s => s.CustomerFacingName))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy));
    }
    public static RewardParameterApiModelBase MapFromRewardTermsToRewardParameters(
        RewardTerms terms, RewardType rewardType)
    {
        return RewardParameterMappingHelper.MapFromRewardTermsToRewardParameters(terms, rewardType);
    }

    public static RewardReloadConfig MapFromRewardParametersToRewardReload(RewardParameterApiModelBase rewardParameterApiModel,
        string rewardType)
    {
        return RewardParameterMappingHelper.MapFromRewardParametersToRewardReload(rewardParameterApiModel, rewardType);
    }
    public static Dictionary<string, string> MapFromRewardParametersToDictionary(
        RewardParameterApiModelBase rewardParameterApiModel,
        string rewardType)
    {
        return RewardParameterMappingHelper.MapFromRewardParametersToDictionary(rewardParameterApiModel, rewardType);
    }
}
