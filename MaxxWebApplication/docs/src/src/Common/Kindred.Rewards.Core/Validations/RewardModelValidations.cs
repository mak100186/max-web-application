using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;

using Newtonsoft.Json;

namespace Kindred.Rewards.Core.Validations;

public static class RewardModelValidations
{
    public static void ThrowIfReloadConfigurationsAreInvalid(this RewardDomainModel reward)
    {
        var reload = reward.Terms.Restrictions.Reload;

        if (!reward.Terms.Restrictions.ClaimsPerInterval.HasValue)
        {
            if (reload == null)
            {
                return;
            }

            throw new RewardsValidationException(
                $"Reload configuration is not required if {nameof(reward.Terms.Restrictions.ClaimsPerInterval)} is null");
        }

        if (reload == null)
        {
            throw new RewardsValidationException(
                $"Reload configuration is required if {nameof(reward.Terms.Restrictions.ClaimsPerInterval)} is not null");
        }

        if (reload.MaxReload is < 1)
        {
            throw new RewardsValidationException($"{nameof(reload.MaxReload)} should be null or greater than 0");
        }

        if (reload.StopOnMinimumWinBets < 1)
        {
            throw new RewardsValidationException($"{nameof(reload.StopOnMinimumWinBets)} should be greater than 0");
        }
    }

    public static void ThrowIfTagsContainEmptyOrWhiteSpace(this RewardDomainModel reward)
    {
        if (reward.Tags.ElementsContainEmptyOrWhiteSpace())
        {
            throw new RewardsValidationException("Tags can not have spaces and should be of one word only");
        }
    }

    public static void ThrowIfAmountIsNotValid(this RewardDomainModel reward)
    {
        if (reward.Terms.GetAmount() <= 0)
        {
            throw new RewardsValidationException("Amount parameter must be a valid decimal value");
        }
    }

    public static void ThrowIfFormulaeAreInvalid(this RewardDomainModel reward, string key)
    {
        var formulae = reward.Terms.GetAllowedFormulae();
        var validFormulae = FormulaExtensions.GetAllFormulae();

        if (formulae.Except(validFormulae).Any())
        {
            //unknown formulae
            throw new RewardsValidationException($"Reward parameter {key} must contain a valid formula. Valid values are: [{validFormulae.ToCsv()}]");
        }

        reward.ThrowIfOptionalIntegerParameterWasInvalid(RewardParameterKey.MinStages);
        reward.ThrowIfOptionalIntegerParameterWasInvalid(RewardParameterKey.MaxStages);
        reward.ThrowIfOptionalIntegerParameterWasInvalid(RewardParameterKey.MinCombinations);
        reward.ThrowIfOptionalIntegerParameterWasInvalid(RewardParameterKey.MaxCombinations);
    }

    public static void ThrowIfOptionalIntegerParameterWasInvalid(this RewardDomainModel reward, string key)
    {
        if (reward.Terms.RewardParameters.TryGetValue(key, out var valueString) &&
            !int.TryParse(valueString, out _))
        {
            throw new RewardsValidationException($"Reward parameter {key} must contain a valid integer. Value found was: [{valueString}]");
        }
    }

    public static void ThrowIfOptionalDecimalParameterWasInvalid(this RewardDomainModel reward, string key)
    {
        if (reward.Terms.RewardParameters.TryGetValue(key, out var valueString) &&
            !string.IsNullOrWhiteSpace(valueString) && (!decimal.TryParse(valueString, out var value) || value <= 0m))
        {
            throw new RewardsValidationException($"Reward parameter {key} must contain a valid decimal greater than 0. Value found was: [{valueString}]");
        }
    }

    public static void ThrowIfOddsLadderOffsetIsNotAnInteger(this RewardDomainModel reward)
    {
        if (!reward.Terms.RewardParameters.TryGetValue<int>(RewardParameterKey.OddsLadderOffset, out _))
        {
            throw new RewardsValidationException($"{nameof(RewardParameterKey.OddsLadderOffset)} value must be a valid integer");
        }
    }

    public static void ThrowIfLegTableIsInvalid(this RewardDomainModel reward, IReadOnlyCollection<BetTypes> betTypes)
    {
        try
        {
            if (betTypes == null)
            {
                throw new("No BetTypes were passed for validation");
            }

            var table = JsonConvert.DeserializeObject<Dictionary<int, decimal>>(
                reward.Terms.GetLegTable(string.Empty));

            if (table == null || !table.Any())
            {
                throw new($"Leg definitions for the following bet types are expected: [{betTypes.ToCsv()}]");
            }

            foreach (var betType in betTypes)
            {
                switch (betType)
                {
                    case BetTypes.SingleLeg:

                        if (!table.ContainsKey(1))
                        {
                            throw new("For BetTypes.SingleLeg, a leg definition for 1 leg was expected");
                        }

                        break;
                    case BetTypes.StandardMultiLeg:
                    case BetTypes.SystemMultiLeg:

                        if (!table.HasAnyKeys(DomainConstants.MultiBetValidLegs))
                        {
                            throw new($"For BetTypes.{betType}, at least one leg definition for {DomainConstants.MinNumberOfLegsInMulti} and up to {DomainConstants.MaxNumberOfLegsInMulti} was expected");
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unkown {betType} passed for validation");
                }
            }
        }
        catch (Exception e)
        {
            throw new RewardsValidationException($"{RewardParameterKey.LegTable} is invalid. See inner exception", e);
        }
    }

    public static void ThrowIfRequiredParameterKeysAreMissing(
        this RewardDomainModel reward,
        IReadOnlyCollection<string> requiredParameterKeys)
    {
        var requiredParameters = requiredParameterKeys
            .Where(p => !reward.Terms.RewardParameters.Keys.Contains(p, StringComparer.InvariantCultureIgnoreCase))
            .ToList();

        if (requiredParameters.IsNotNullAndNotEmpty())
        {
            throw new RewardsValidationException($"Reward parameter(s) [{requiredParameters.ToCsv()}] is required");
        }
    }

    public static void ThrowIfInvalidParameterKeysAreFound(this RewardDomainModel reward,
        IReadOnlyCollection<string> requiredParameterKeys, IReadOnlyCollection<string> optionalParameterKeys)
    {
        var invalidParameters = reward.Terms.RewardParameters.Keys
            .Where(p => !requiredParameterKeys.Contains(p) && !optionalParameterKeys.Contains(p))
            .ToList();

        if (invalidParameters.Any())
        {
            throw new RewardsValidationException($"Reward Parameter(s) [{invalidParameters.ToCsv()}] is not valid");
        }
    }

    public static void ThrowIfMultiConfigsAreInvalid(this RewardDomainModel reward, IReadOnlyCollection<BetTypes> betTypes, IReadOnlyCollection<BetTypes> allowedBetTypes)
    {
        foreach (var betType in betTypes)
        {
            if (!allowedBetTypes.Contains(betType))
            {
                throw new RewardsValidationException($"Only one of the following bet types are allowed: {allowedBetTypes.ToCsv()}");
            }
        }

        if (betTypes.ContainsMultiBetTypes())
        {
            //if single leg is present then allow for minStages to be 1
            reward.ThrowIfIsOutOfRange(RewardParameterKey.MinStages,
                betTypes.Contains(BetTypes.SingleLeg)
                    ? 1
                    : DomainConstants.MinNumberOfLegsInMulti,
                DomainConstants.MaxNumberOfLegsInMulti);

            reward.ThrowIfIsOutOfRange(RewardParameterKey.MaxStages, DomainConstants.MinNumberOfLegsInMulti, DomainConstants.MaxNumberOfLegsInMulti);

            reward.ThrowIfMaxIsLessThanMinNumberOfLegs();
        }
    }

    public static void ThrowIfIsOutOfRange(this RewardDomainModel reward, string key, int minValue, int maxValue)
    {
        if (!reward.Terms.IsKeyPresent(key))
        {
            return;
        }

        if (reward.Terms.RewardParameters.TryGetValue(key, out int valueUnderTest))
        {
            if (valueUnderTest < minValue || valueUnderTest > maxValue)
            {
                throw new RewardsValidationException($"{key} must be between {minValue} and {maxValue}");
            }
        }
        else
        {
            throw new RewardsValidationException($"{key} contains an invalid value");
        }
    }

    public static void ThrowIfMaxIsLessThanMinNumberOfLegs(this RewardDomainModel reward)
    {
        var minStages = reward.Terms.GetMinStages();
        var maxStages = reward.Terms.GetMaxStages();

        if (minStages > 0 && maxStages > 0 && minStages > maxStages)
        {
            throw new RewardsValidationException($"{RewardParameterKey.MinStages} must be less than {RewardParameterKey.MaxStages}");
        }
    }
}
