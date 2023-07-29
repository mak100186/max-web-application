using AutoMapper;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Plugin.Reward.Models;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public class RewardRequestBuilder
{
    public static T CreateRewardRequestForType<T>(RewardType rewardType, Action<T> callback)
        where T : RewardRequest, new()
    {
        callback = SetCallbackIfNull(callback);

        var request = Create<T>(rewardType)
            .UpdateRewardBaseProps()
            .UpdateRewardClassSpecificProps(rewardType);

        callback?.Invoke(request);

        return request;
    }

    private static T Create<T>(RewardType rewardType)
        where T : RewardRequest, new()
    {
        var terms = RewardBaseDomainModelBuilder.GetTerms(rewardType);

        var request = new T
        {
            RewardType = rewardType.ToString(),

            CustomerFacingName = "DefaultCustomerFacingName",
            Name = "DefaultRewardName",

            CreatedBy = "ModelBuilder",
            UpdatedBy = "ModelBuilder",

            Attributes = new(),
            Tags = new List<string> { "DefaultTag" },

            RewardParameters = RewardParameterMappingHelper.MapFromRewardTermsToRewardParameters(terms, rewardType),
            Restrictions = WithProfile<RewardDomainToApiMapping>.Map<RewardRestrictionApiModel>(terms.Restrictions),
            DomainRestriction = WithProfile<RewardDomainToApiMapping>.Map<DomainRestrictionApiModel>(terms),

            State = "SomeState",
            CountryCode = "AUS"
        };

        return request;
    }

    private static Action<T> SetCallbackIfNull<T>(Action<T> callback)
        where T : RewardRequest
    {
        callback ??= _ => { };
        return callback;
    }
}

public static class WithProfile<TProfile>
    where TProfile : Profile, new()
{
    private static readonly Lazy<IMapper> s_mapperFactory = new(() =>
    {
        MapperConfiguration provider = new(config => config.AddProfile<TProfile>());
        return new Mapper(provider);
    });

    public static IMapper Mapper => s_mapperFactory.Value;


    public static TDestination Map<TDestination>(object source)
    {
        return Mapper.Map<TDestination>(source);
    }
}
