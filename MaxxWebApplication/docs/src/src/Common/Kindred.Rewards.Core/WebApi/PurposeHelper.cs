using Kindred.Rewards.Core.WebApi.Enums;

namespace Kindred.Rewards.Core.WebApi;

public static class PurposeHelper
{
    public static readonly IReadOnlyCollection<SubPurposeType> SubPurposeCrm = new[]
    {
        SubPurposeType.Acquisition,
        SubPurposeType.Lifecycle,
        SubPurposeType.ReoccurringReward,
        SubPurposeType.EventReward,
        SubPurposeType.Compensation
    };

    public static readonly IReadOnlyCollection<SubPurposeType> SubPurposeCustomerService = new[]
    {
        SubPurposeType.BonusRequestAcquisition,
        SubPurposeType.BonusRequestRetention,
        SubPurposeType.ServiceCompensation,
        SubPurposeType.GoodwillGesture
    };

    public static readonly IReadOnlyCollection<SubPurposeType> SubPurposeVipManager = new[]
    {
        SubPurposeType.BonusRequestAcquisition,
        SubPurposeType.BonusRequestRetention,
        SubPurposeType.ServiceCompensation,
        SubPurposeType.GoodwillGesture
    };

    public static readonly IReadOnlyCollection<SubPurposeType> SubPurposeAffiliate = new[]
    {
        SubPurposeType.Acquisition,
        SubPurposeType.Lifecycle,
        SubPurposeType.RecurringReward,
        SubPurposeType.EventReward,
        SubPurposeType.BonusRequestAcquisition,
        SubPurposeType.BonusRequestRetention
    };

    public static readonly IReadOnlyCollection<SubPurposeType> SubPurposeBdm = new[]
    {
        SubPurposeType.Acquisition,
        SubPurposeType.RecurringDeal,
        SubPurposeType.BonusRequestAcquisition,
        SubPurposeType.BonusRequestRetention
    };


    public static IReadOnlyCollection<SubPurposeType> GetSubPurpose(PurposeType purpose)
    {
        return purpose switch
        {
            PurposeType.Crm => SubPurposeCrm,
            PurposeType.Customerservice => SubPurposeCustomerService,
            PurposeType.Vipmanager => SubPurposeVipManager,
            PurposeType.Affiliate => SubPurposeAffiliate,
            PurposeType.Bdm => SubPurposeBdm,
            _ => throw new("Invalid Purpose")
        };
    }
}
