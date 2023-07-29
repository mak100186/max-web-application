using System.ComponentModel.DataAnnotations;

namespace Kindred.Rewards.Core.WebApi.Validation.Enums;

public enum RewardErrorCodes
{
    /// <summary>
    /// Promotion name is required
    /// </summary>
    [Display(Description = "Name is required")]
    RewardNameIsRequired = 201,

    /// <summary>
    /// Promotion description is required
    /// </summary>
    [Display(Description = "Promotion description is required")]
    PromotionDescriptionIsRequired = 202,

    /// <summary>
    /// Reward type is invalid
    /// </summary>
    [Display(Description = "Reward type is either empty or invalid")]
    RewardTypeIsInvalid = 204,

    /// <summary>
    /// Restrictions is required
    /// </summary>
    [Display(Description = "Restrictions is required")]
    RestrictionsIsRequired = 205,

    /// <summary>
    /// RewardParameters is required
    /// </summary>
    [Display(Description = "RewardParameters is required")]
    RewardParametersIsRequired = 207,

    /// <summary>
    /// Claim interval is not valid
    /// </summary>
    [Display(Description = "Claim interval cron expression is not valid")]
    ClaimIntervalExpressionIsNotValid = 208,

    /// <summary>
    /// Start date should be less than expiry date
    /// </summary>
    [Display(Description = "Start date should be less than expiry date")]
    StartDateShouldBeLessThanExpiryDate = 209,

    /// <summary>
    /// Expiry date should be greater than start date
    /// </summary>
    [Display(Description = "Expiry date should be greater than start date")]
    ExpiryDateShouldBeGreaterThanStartDate = 210,

    /// <summary>
    /// Expiry date should be greater than current date time
    /// </summary>
    [Display(Description = "Expiry date should be greater than current date time")]
    ExpiryDateShouldBeGreaterThanUtcNow = 211,

    /// <summary>
    /// Claim per interval cannot be zero
    /// </summary>
    [Display(Description = "Claim per interval cannot be zero")]
    ClaimPerIntervalCannotBeZero = 212,

    /// <summary>
    /// Promotion name's maximum length is 100
    /// </summary>
    [Display(Description = "Reward name's maximum length is 100")]
    NameMaxLengthIs100 = 215,

    /// <summary>
    /// Promotion description's maximum length is 300
    /// </summary>
    [Display(Description = "Reward description's maximum length is 300")]
    DescriptionMaxLengthIs300 = 216,

    /// <summary>
    /// Start date should be less than DateTime.MaxValue
    /// </summary>
    [Display(Description = "Start date should be less than DateTime.MaxValue")]
    StartDateShouldBeLessThanMaxDate = 218,

    /// <summary>
    /// Claim interval is not valid
    /// </summary>
    [Display(Description = "Claim allowed period cron expression is not valid")]
    ClaimAllowedPeriodExpressionIsNotValid = 219,

    /// <summary>
    /// Claim interval is not valid
    /// </summary>
    [Display(Description = "Name should not contain any special character")]
    RewardNameShouldNotContainInvalidChar = 220,

    /// <summary>
    /// Name should not have leading or trailing dot character
    /// </summary>
    [Display(Description = "Name should not have leading or trailing dot character")]
    RewardNameShouldNotContainLeadingOrTrailingDot = 222,

    /// <summary>
    /// Time zone Id must be valid
    /// </summary>
    [Display(Description = "TimezoneId must be valid")]
    TimezoneIdShouldBeValid = 224,

    /// <summary>
    /// Claim allowed period is required when time zone id is given
    /// </summary>
    [Display(Description = "ClaimAllowedPeriod is required when timezoneId is given")]
    ClaimAllowedPeriodIsRequiredWhenTimezoneIsGiven = 225,

    /// <summary>
    /// CustomerId is required
    /// </summary>
    [Display(Description = "CustomerId is required")]
    CustomerIdIsRequired = 226,
    /// <summary>
    /// Reward Parameter type must match Reward type
    /// </summary>
    [Display(Description = "Reward Parameter type must match Reward type")]
    RewardParameterTypeMustMatchRewardType = 227,

    /// <summary>
    /// MinimumCompoundOdds and MinimumStageOdds cannot both be supplied
    /// </summary>
    [Display(Description = "MinimumCompoundOdds and MinimumStageOdds cannot both be supplied")]
    MinimumCompoundAndStageOddsShouldBeExclusive = 228,

    /// <summary>
    /// State has a maximum length of 50 characters
    /// </summary>
    [Display(Description = "State has a maximum length of 50 characters")]
    StateHasMaximumLengthOfFiftyCharacters = 229,

    /// <summary>
    /// Rewards are only sortable on certain fields
    /// </summary>
    [Display(Description = "Rewards are only sortable on certain fields - ")]
    RewardsAreOnlySortableOnCertainFields = 230,

    /// <summary>
    /// Currency code has a maximum length of 20
    /// </summary>
    [Display(Description = "Currency code has a maximum length of 20")]
    CurrencyHasAMaximumLengthOfTwenty = 231,
}
