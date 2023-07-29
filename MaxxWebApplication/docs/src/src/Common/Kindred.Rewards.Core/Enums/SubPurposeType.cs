using System.ComponentModel.DataAnnotations;

namespace Kindred.Rewards.Core.Enums;

public enum SubPurposeType
{
    [Display(Description = "Acquisition")]
    Acquisition,

    [Display(Description = "Lifecycle")]
    Lifecycle,

    [Display(Description = "Reoccurring Reward")]
    ReoccurringReward,

    [Display(Description = "Event Reward")]
    EventReward,

    [Display(Description = "Compensation")]
    Compensation,

    [Display(Description = "Bonus Request Acquisition")]
    BonusRequestAcquisition,

    [Display(Description = "Bonus Request Retention")]
    BonusRequestRetention,

    [Display(Description = "Service Compensation")]
    ServiceCompensation,

    [Display(Description = "Goodwill Gesture")]
    GoodwillGesture,

    [Display(Description = "Recurring Reward")]
    RecurringReward,

    [Display(Description = "Recurring Deal")]
    RecurringDeal
}
