using System.Security.Cryptography;
using System.Text;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;

namespace Kindred.Rewards.Core.Extensions;
public static class RewardDomainModelExtensions
{
    public static string GetSha256(this RewardDomainModel model)
    {
        var rn = model.RewardId;

        var restrictions = model.Terms.Restrictions;

        var domainRestrictionParams = new[] { RewardParameterKey.MinStages, RewardParameterKey.MaxStages, RewardParameterKey.MinCombinations, RewardParameterKey.MaxCombinations };

        var dateTimeRestrictions = $"{restrictions.TimezoneId}-{restrictions.StartDateTime.Ticks}-{restrictions.ExpiryDateTime.Ticks}-{restrictions.ClaimInterval}-{restrictions.ClaimAllowedPeriod}";

        var platformRestrictions = $"{model.CountryCode}-{model.Jurisdiction}-{model.Brand}-{model.State}";

        var domainRestrictions = model.Terms.RewardParameters.Where(kvp => domainRestrictionParams.Contains(kvp.Key)).Select((k, v) => $"{k}:{v}").ToCsv();

        var rewardParameters = model.Terms.RewardParameters.Where(kvp => !domainRestrictionParams.Contains(kvp.Key)).Select((k, v) => $"{k}:{v}").ToCsv();

        var reward = $"{rn}-{dateTimeRestrictions}-{platformRestrictions}-{domainRestrictions}-{rewardParameters}";

        var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(reward);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }

    public static void SetDefaultMultiConfigRewardParameters(this RewardDomainModel model,
        IDictionary<string, string> defaults)
    {
        if (DoesNotExistInRewardAndHasDefault(model, defaults, RewardParameterKey.MinStages))
        {
            model.Terms.RewardParameters.Add(RewardParameterKey.MinStages, defaults[RewardParameterKey.MinStages]);
        }

        if (DoesNotExistInRewardAndHasDefault(model, defaults, RewardParameterKey.MaxStages))
        {
            model.Terms.RewardParameters.Add(RewardParameterKey.MaxStages, defaults[RewardParameterKey.MaxStages]);
        }

        if (DoesNotExistInRewardAndHasDefault(model, defaults, RewardParameterKey.MinCombinations))
        {
            model.Terms.RewardParameters.Add(RewardParameterKey.MinCombinations, defaults[RewardParameterKey.MinCombinations]);
        }

        if (DoesNotExistInRewardAndHasDefault(model, defaults, RewardParameterKey.MaxCombinations))
        {
            model.Terms.RewardParameters.Add(RewardParameterKey.MaxCombinations, defaults[RewardParameterKey.MaxCombinations]);
        }

        if (DoesNotExistInRewardAndHasDefault(model, defaults, RewardParameterKey.AllowedFormulae))
        {
            model.Terms.RewardParameters.Add(RewardParameterKey.AllowedFormulae, defaults[RewardParameterKey.AllowedFormulae]);
        }
    }

    private static bool DoesNotExistInRewardAndHasDefault(RewardDomainModel model, IDictionary<string, string> defaults, string key)
    {
        return !model.Terms.RewardParameters.ContainsKey(key) &&
               defaults.ContainsKey(key);
    }
}
