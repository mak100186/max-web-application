using System.Diagnostics.CodeAnalysis;

namespace Kindred.Rewards.Core;

[ExcludeFromCodeCoverage]
public static class DomainConstants
{
    public const int MaxNumberOfLegsInMulti = 20;
    public const int MinNumberOfLegsInMulti = 2;

    public const int MaxNumberOfCombinationsInMulti = 4845; //taken from multi-lib
    public const int MinNumberOfCombinationsInMulti = 1;

    //the number of legs that are valid for a multi bet of any kind: standard, formula, etc. 
    public static IReadOnlyCollection<int> MultiBetValidLegs => Enumerable.Range(MinNumberOfLegsInMulti, MaxNumberOfLegsInMulti - MinNumberOfLegsInMulti).ToList();

    public const string InfiniteCronInterval = "0 0 0 L DEC ? 2099";
    public const string Every30SecondsCronInterval = "0/30 * * ? * * *";

    #region Api

    public const string RewardsApiErrorCodePrefix = "RewardsApi";
    public const string GetRewardRoute = "GetRewardRoute";

    #endregion

    #region Database

    public static readonly string ConnectionString = "RewardsContext";
    public const string ShouldDropDbBetweenBuilds = "appsettings:ShouldDropDbBetweenBuilds";
    public const string OfferPriceManagerBaseUrl = "appsettings:OfferPriceManagerBaseUrl";
    public const string RewardsMarketMirrorBaseUrl = "appsettings:RewardsMarketMirrorBaseUrl";
    public const string WebApiUrl = "appsettings:webApiUrl";
    public const string ShouldFetchOnDemand = "appsettings:ShouldFetchOddsLadderOnDemand";

    #endregion

    public const string TemplateBlueprints = "blueprints";

    public const char SortByDescendingIndicator = '-';

    #region Kafka
    public const string TopicProfileClaimUpdates = "ClaimUpdates";
    public const string TopicProfileTemplateUpdates = "TemplateUpdates";
    public const string TopicProfileRewardUpdates = "RewardUpdates";
    public const string TopicProfileOddsLadder = "OddsLadder";
    public const string TopicProfileBetUpdates = "BetUpdates";
    public const string TopicProfileMissionsAchieved = "MissionsAchieved";

    public const string AutoCreatedTagComments = "Auto-created tag...";

    public const string DefaultCurrencyCode = "AUD";
    public const string DefaultThreeLetterCountryCode = "AUS";
    #endregion

    #region Odds Ladder

    public static string GetOddsLadderContestCacheKey(string contestType) => $"oddsladder-{contestType}";

    #endregion

    #region SemanticResourceNaming

    public static class Rn
    {
        public const string Separator = ":";
        public const int DefaultSchemeVersion = 1;

        public static class Formats
        {
            public static class Elements
            {
                public const string Namespace = "{namespace}";
                public const string EntitySegment = "{entity_segment}";
                public const string EntityType = "{entity_type}";
                public const string SchemeVersion = "{scheme_version}";
                public const string EntityScheme = $"{EntityType}.{SchemeVersion}";
            }
            public const string Parent = $"{Elements.Namespace}{Separator}{Elements.EntityScheme}{Separator}{Elements.EntitySegment}";
        }


        public static class Namespaces
        {
            public const string Ksp = "ksp";
            public const string Ki = "ki";
            public const string Krp = "krp";
        }

        public static class EntityTypes
        {
            public const string Coupon = "coupon";
            public const string Bet = "bet";
            public const string Market = "market";
            public const string Outcome = "outcome";
            public const string Combination = "combination";
            public const string Reward = "reward";
            public const string Claim = "claim";
            public const string Variant = "variant";
        }

        public static class RegExps
        {
            public const string Guid = "^[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}$";
            public const string AlphaWithAllowedSpecialChars = "^[a-zA-Z_]*$";
            public const string UnsignedNumbersWithoutDecimals = "^[0-9]+$";
        }
    }



    #endregion
}
