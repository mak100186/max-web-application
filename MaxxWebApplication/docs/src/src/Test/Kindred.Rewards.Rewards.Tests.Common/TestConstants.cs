using Kindred.Rewards.Core.Models.Rn;

namespace Kindred.Rewards.Rewards.Tests.Common;

public static class TestConstants
{
    public static RewardRn RewardRn;

    static TestConstants()
    {
        RewardRn = new(Guid.NewGuid());
    }

    public const string TestPrefix = "QhePSHBDc3hQUQv57OuF";

    // entitlement table column headers
    public const string DefaultMarketRn = "ksp:market.1:[football:201711202200:watford_vs_west_ham]:1x2";
    public const string DefaultContestType = "football";
    public const string RestResponseSharedState = nameof(RestResponseSharedState);
    public const string RestResponseSharedError = nameof(RestResponseSharedError);
    public const string RestResponse = nameof(RestResponse);

    public const string ExpiryDaysFromNow = "expiryDaysFromNow";
    public const string StartDaysFromNow = "startDaysFromNow";
    public const string EtcUtcTimeZone = "Etc/UTC";
    public const string TimeInNextDaylightSavingTimeShift = "time-in-next-daylight-saving-time-shift";

    public const string DefaultCountryCode = "AUS";
    public const string DefaultCurrencyCode = "AUD";
    public const string DefaultCustomerId = "12345";
    public const string DefaultPromotionComments = "TestPromotion";
}
