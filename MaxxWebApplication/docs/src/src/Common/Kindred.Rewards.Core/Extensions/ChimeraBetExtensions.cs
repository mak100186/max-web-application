using System.Text.RegularExpressions;

namespace Kindred.Rewards.Core.Extensions;
public static class ChimeraBetExtensions
{
    private const string ContestKeyRegex = "ksp:.*\\[(.*)\\].*";

    public static string GetContestKeyFromMarket(this string market)
    {
        var match = Regex.Match(market, ContestKeyRegex);

        if (!match.Success)
        {
            return null; //throw new ArgumentException($"Unable to parse contestKey from {market}", nameof(market));
        }

        var contestKey = match.Groups[1].Value; // Groups[0] represents the entire input string

        return contestKey;
    }

    public static string GetContestTypeFromMarket(this string market)
    {
        var contestKey = market.GetContestKeyFromMarket();

        var contestSegments = contestKey?.Split(":");

        return contestSegments?.First();
    }
}
