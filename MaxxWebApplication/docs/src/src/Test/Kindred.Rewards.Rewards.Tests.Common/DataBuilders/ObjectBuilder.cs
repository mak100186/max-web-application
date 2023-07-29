using System.Security.Cryptography;
using System.Text;

using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;
using Kindred.Rewards.Core.WebApi.Payloads.BetModel;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public class ObjectBuilder
{
    private const string SeedString = "hNoHZUOVLQEmKRLYJblspOqNbiKjXYVxMMJmztYIzOUlBJZaCnqlGzTjDUvYxiNtAKfXopmhElxgfqaOKstEbZPSDCOHewLlZdbGmQTwBBjptiskOrgFnVpmGOObLFyUOMGfrsRdmBOwPwixVtpLcetclbsqNrBFlujXspIfpcbJDjjLbkneqNMOGJiLkyoizKbjTixq";

    private static readonly string[] s_contestTypes = { "football", "golf", "basketball" };
    private static readonly string[] s_propositionTypes = { "1x2" };

    public static string CreateString(int length = 36, bool useGuid = true)
    {
        var sb = new StringBuilder(useGuid ? Guid.NewGuid().ToString() : SeedString);

        while (sb.Length < length)
        {
            sb.Append(useGuid ? $"+{Guid.NewGuid()}" : SeedString);
        }

        return sb.ToString()[..length];
    }

    public static List<string> CreateManyStrings(int count = 3, int length = 36, bool useGuid = true)
    {
        return LoopUtilities.CreateMany(() => CreateString(length, useGuid), count);
    }

    public static List<string> GetCurrencyCodes()
    {
        return RegionHelper.GetCountriesByIso3166().Select(x => x.ISOCurrencySymbol).ToList();
    }

    public static string CreateMarketKey(string teamA = null, string teamB = null, string contestKey = null)
    {
        contestKey = CreateContestKey(teamA, teamB, contestKey);

        var propositionType = s_propositionTypes[RandomNumberGenerator.GetInt32(s_propositionTypes.Length)];

        return $"ksp:market.1[{contestKey}]:{propositionType}";
    }

    public static string CreateContestKey(string teamA = null, string teamB = null, string contestKey = null, string sport = null)
    {
        teamA ??= CreateString(10);
        teamB ??= CreateString(10);

        sport ??= s_contestTypes[RandomNumberGenerator.GetInt32(s_contestTypes.Length)];
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddhhmm");

        contestKey ??= $"{sport}:{timestamp}:{teamA}_vs_{teamB}";

        return contestKey;
    }

    public static BetDomainModel CreateBet(int stages = 1, decimal stake = 10.0m, decimal odds = 1.5m, string[] marketKeys = null, string[] contestKeys = null, string[] outcomes = null)
    {
        return new()
        {
            Formula = "singles",
            RequestedStake = stake,
            Rn = CreateString(),
            Status = "pending",
            Stages = Enumerable.Range(1, stages).Select(i =>
                new CompoundStageDomainModel
                {
                    Market = marketKeys != null ? marketKeys[i - 1] : CreateMarketKey(contestKey: contestKeys != null ? contestKeys[i - 1] : null),
                    RequestedOutcome = outcomes != null ? outcomes[i - 1] : CreateString(),
                    RequestedPrice = odds
                }).ToList()
        };
    }

    public static RewardClaimDomainModel CreateRewardClaim(RewardType rewardType, int stages, decimal stake)
    {
        var bet = CreateBet(stages, stake);

        return new()
        {
            Bet = bet,
            BetOutcomeStatus = null,
            BetRn = bet.Rn,
            Terms = RewardBaseDomainModelBuilder.GetTerms(rewardType)
        };
    }

    public static BatchClaimItemDomainModel CreateBatchClaimItemDomainModel(BetDomainModel bet, string hash, string rewardId)
    {
        return new()
        {
            Bet = bet,
            Hash = hash,
            RewardId = rewardId
        };
    }

    public static BetApiModel CreateApiBet(int stages = 1, decimal stake = 10.0m, decimal odds = 1.5m, string[] marketKeys = null, string[] contestKeys = null, string[] outcomes = null)
    {
        return new()
        {
            Formula = "none",
            RequestedStake = stake,
            Rn = CreateString(),
            Status = "pending",
            Stages = Enumerable.Range(1, stages).Select(i =>
                new CompoundStageApiModel
                {
                    Market = marketKeys != null ? marketKeys[i - 1] : CreateMarketKey(contestKey: contestKeys?[i - 1]),
                    RequestedSelection = new()
                    { Outcome = outcomes != null ? outcomes[i - 1] : CreateString() },
                    Odds = new()
                    { RequestedPrice = odds }
                }).ToList()
        };
    }

    public static string GetRandomCurrencyCode()
    {
        var currencyCodes = GetCurrencyCodes();

        return !currencyCodes.Any()
            ? DomainConstants.DefaultCurrencyCode
            : currencyCodes.ElementAt(RandomNumberGenerator.GetInt32(0, currencyCodes.Count));
    }

    public static T GetRandomEnum<T>()
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }

        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(RandomNumberGenerator.GetInt32(0, values.Length));
    }

}
