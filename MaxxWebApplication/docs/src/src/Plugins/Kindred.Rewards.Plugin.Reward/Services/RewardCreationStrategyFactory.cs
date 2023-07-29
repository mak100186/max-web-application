using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Validations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Reward.Services;


public interface IRewardCreationStrategy
{
    IReadOnlyCollection<string> RequiredParameterKeys { get; }

    IReadOnlyCollection<string> OptionalParameterKeys { get; }

    bool ValidateAndInitialise(RewardDomainModel reward);
}


public interface IRewardCreationStrategyFactory
{
    IRewardCreationStrategy GetRewardCreationStrategy(RewardType rewardType);
}

public class RewardCreationStrategyFactory : IRewardCreationStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RewardCreationStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IRewardCreationStrategy GetRewardCreationStrategy(RewardType rewardType)
    {
        return rewardType switch
        {
            RewardType.Freebet => _serviceProvider.GetService<FreeBetCreationStrategy>(),
            RewardType.Uniboost => _serviceProvider.GetService<UniBoostCreationStrategy>(),
            RewardType.UniboostReload => _serviceProvider.GetService<UniBoostReloadCreationStrategy>(),
            RewardType.Profitboost => _serviceProvider.GetService<ProfitBoostCreationStrategy>(),
            _ => null
        };
    }
}

public class RewardCreationStrategyBase : IRewardCreationStrategy
{
    protected readonly ILogger Logger;

    public string RewardName { get; protected set; }

    protected RewardCreationStrategyBase(ILogger logger)
    {
        Logger = logger;

        // Features are the common behaviors that can be applied to any reward
        Features = new()
        {
            {FeatureType.Reload, new ReloadFeature(logger)}
        };
    }

    public Dictionary<FeatureType, IFeature> Features { get; }

    public virtual IReadOnlyCollection<string> RequiredParameterKeys => new List<string>();

    public virtual IReadOnlyCollection<string> OptionalParameterKeys => new List<string>();

    public virtual bool ValidateAndInitialise(RewardDomainModel reward)
    {
        reward.ThrowIfTagsContainEmptyOrWhiteSpace();

        foreach (var feature in Features)
        {
            feature.Value.ApplyCreationRules(reward);
        }

        reward.ThrowIfRequiredParameterKeysAreMissing(RequiredParameterKeys);

        reward.ThrowIfInvalidParameterKeysAreFound(RequiredParameterKeys, OptionalParameterKeys);

        return true;
    }
}

public class FreeBetCreationStrategy : RewardCreationStrategyBase
{
    public FreeBetCreationStrategy(ILogger<FreeBetCreationStrategy> logger) : base(logger)
    {
    }

    public override IReadOnlyCollection<string> RequiredParameterKeys => new List<string>
    {
        RewardParameterKey.Amount,
    };

    public override IReadOnlyCollection<string> OptionalParameterKeys => new List<string>
    {
        RewardParameterKey.MinStages,
        RewardParameterKey.MaxStages,
        RewardParameterKey.MinCombinations,
        RewardParameterKey.MaxCombinations,
        RewardParameterKey.AllowedFormulae,
        RewardParameterKey.MaxExtraWinnings
    };

    public static IDictionary<string, string> DefaultParameterKeys => new Dictionary<string, string>
    {
        { RewardParameterKey.Amount, "10" },
        { RewardParameterKey.MinStages, "1" },
        { RewardParameterKey.MaxStages, $"{DomainConstants.MaxNumberOfLegsInMulti}" },
        { RewardParameterKey.MinCombinations, $"{DomainConstants.MinNumberOfCombinationsInMulti}" },
        { RewardParameterKey.MaxCombinations, $"{DomainConstants.MaxNumberOfCombinationsInMulti}" },
        { RewardParameterKey.AllowedFormulae, AllowedFormulae.ToCsv() },
        { RewardParameterKey.MaxExtraWinnings, null },
    };

    public static IReadOnlyCollection<BetTypes> AllowedBetTypes = new List<BetTypes>
    {
        BetTypes.SingleLeg,
        BetTypes.StandardMultiLeg,
        BetTypes.SystemMultiLeg
    };

    public static IReadOnlyCollection<string> AllowedFormulae = FormulaExtensions.GetFormulaeForBetTypes(AllowedBetTypes);

    public static RewardSettlement DefaultSettlementTerms => new()
    {
        ReturnStake = false
    };

    public override bool ValidateAndInitialise(RewardDomainModel reward)
    {
        base.ValidateAndInitialise(reward);

        reward.SetDefaultMultiConfigRewardParameters(DefaultParameterKeys);

        reward.ThrowIfAmountIsNotValid();

        reward.ThrowIfOptionalDecimalParameterWasInvalid(RewardParameterKey.MaxExtraWinnings);

        reward.ThrowIfFormulaeAreInvalid(RewardParameterKey.AllowedFormulae);

        var betTypes = reward.Terms.RewardParameters.GetApplicableBetTypes();

        reward.ThrowIfMultiConfigsAreInvalid(betTypes, AllowedBetTypes);

        reward.Terms.SettlementTerms = new() { ReturnStake = false };

        return true;
    }
}


public class UniBoostCreationStrategy : RewardCreationStrategyBase
{
    public UniBoostCreationStrategy(ILogger<UniBoostCreationStrategy> logger)
        : base(logger)
    {
        RewardName = nameof(RewardType.Uniboost);
    }

    public override IReadOnlyCollection<string> OptionalParameterKeys => new List<string>
    {
        RewardParameterKey.MaxExtraWinnings,
        RewardParameterKey.MaxStakeAmount,
    };

    public override IReadOnlyCollection<string> RequiredParameterKeys => new List<string>
    {
        RewardParameterKey.OddsLadderOffset,
    };

    public static IDictionary<string, string> DefaultParameterKeys => new Dictionary<string, string>
    {
        { RewardParameterKey.OddsLadderOffset, "3" },
        { RewardParameterKey.MaxStakeAmount, "3" },
        { RewardParameterKey.MaxExtraWinnings, null }
    };

    public static RewardSettlement DefaultSettlementTerms => new() { ReturnStake = false };

    public static IReadOnlyCollection<BetTypes> AllowedBetTypes = new List<BetTypes> { BetTypes.SingleLeg };

    public static IReadOnlyCollection<string> AllowedFormulae = FormulaExtensions.GetFormulaeForBetTypes(AllowedBetTypes);

    public override bool ValidateAndInitialise(RewardDomainModel reward)
    {
        base.ValidateAndInitialise(reward);

        reward.SetDefaultMultiConfigRewardParameters(DefaultParameterKeys);

        reward.ThrowIfOptionalDecimalParameterWasInvalid(RewardParameterKey.MaxStakeAmount);

        reward.ThrowIfOptionalDecimalParameterWasInvalid(RewardParameterKey.MaxExtraWinnings);

        reward.ThrowIfOddsLadderOffsetIsNotAnInteger();

        reward.Terms.SettlementTerms = new() { ReturnStake = false };

        return true;
    }

}

public class UniBoostReloadCreationStrategy : UniBoostCreationStrategy
{
    public UniBoostReloadCreationStrategy(ILogger<UniBoostReloadCreationStrategy> logger)
        : base(logger)
    {
        Features[FeatureType.Reload].Enabled = true;
        RewardName = nameof(RewardType.UniboostReload);
    }
}


public class ProfitBoostCreationStrategy : RewardCreationStrategyBase
{
    public ProfitBoostCreationStrategy(ILogger<ProfitBoostCreationStrategy> logger)
        : base(logger)
    {
        RewardName = nameof(RewardType.Profitboost);
    }

    public override IReadOnlyCollection<string> RequiredParameterKeys => new List<string> { RewardParameterKey.LegTable };

    public override IReadOnlyCollection<string> OptionalParameterKeys => new List<string>
    {
        RewardParameterKey.MinStages,
        RewardParameterKey.MaxStages,
        RewardParameterKey.MinCombinations,
        RewardParameterKey.MaxCombinations,
        RewardParameterKey.AllowedFormulae,
        RewardParameterKey.MaxExtraWinnings,
        RewardParameterKey.MaxStakeAmount
    };

    public static IDictionary<string, string> DefaultParameterKeys => new Dictionary<string, string>
    {
        { RewardParameterKey.MaxStakeAmount, "3" },
        { RewardParameterKey.MinStages, "1" },
        { RewardParameterKey.MaxStages, $"{DomainConstants.MaxNumberOfLegsInMulti}" },
        { RewardParameterKey.MinCombinations, $"{DomainConstants.MinNumberOfCombinationsInMulti}" },
        { RewardParameterKey.MaxCombinations, $"{DomainConstants.MaxNumberOfCombinationsInMulti}" },
        { RewardParameterKey.AllowedFormulae, AllowedFormulae.ToCsv() },
        { RewardParameterKey.LegTable, "{\"1\":\"10\",\"2\":\"10\",\"4\":\"15\",\"7\":\"20\",\"10\":\"0\"}" },
        { RewardParameterKey.MaxExtraWinnings, null }
    };

    public static IReadOnlyCollection<BetTypes> AllowedBetTypes = new List<BetTypes> { BetTypes.SingleLeg, BetTypes.StandardMultiLeg };

    public static IReadOnlyCollection<string> AllowedFormulae = FormulaExtensions.GetFormulaeForBetTypes(AllowedBetTypes);

    public static RewardSettlement DefaultSettlementTerms => new() { ReturnStake = false };

    public override bool ValidateAndInitialise(RewardDomainModel reward)
    {
        base.ValidateAndInitialise(reward);

        reward.SetDefaultMultiConfigRewardParameters(DefaultParameterKeys);

        reward.ThrowIfOptionalDecimalParameterWasInvalid(RewardParameterKey.MaxStakeAmount);

        reward.ThrowIfOptionalDecimalParameterWasInvalid(RewardParameterKey.MaxExtraWinnings);

        reward.ThrowIfFormulaeAreInvalid(RewardParameterKey.AllowedFormulae);

        var betTypes = reward.Terms.RewardParameters.GetApplicableBetTypes();

        reward.ThrowIfMultiConfigsAreInvalid(betTypes, AllowedBetTypes);

        reward.ThrowIfLegTableIsInvalid(betTypes);

        reward.Terms.SettlementTerms = new() { ReturnStake = false };

        return true;
    }

}
