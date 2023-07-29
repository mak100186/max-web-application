using System.ComponentModel.DataAnnotations;

namespace Kindred.Rewards.Core.WebApi.Validation.Enums;

public enum ClaimErrorCodes
{
    /// <summary>
    /// CustomerId is required
    /// </summary>
    [Display(Description = "CustomerId is required")]
    CustomerIdIsRequired = 301,

    /// <summary>
    /// Bet client reference is required
    /// </summary>
    [Display(Description = "CouponRn is required")]
    CouponRnIsRequired,

    /// <summary>
    /// Reward key is required
    /// </summary>
    [Display(Description = "Reward Rn is required")]
    RewardRnIsRequired,

    /// <summary>
    /// Reward class is invalid
    /// </summary>
    [Display(Description = "RewardClass is invalid. Must be either Bonus or Promotion")]
    RewardClassInvalid,

    /// <summary>
    /// Bet reference should be greater than 0
    /// </summary>
    [Display(Description = "BetRef must be greater than zero")]
    BetRefMustBeGreaterThanZero,

    /// <summary>
    /// Market products is required
    /// </summary>
    [Display(Description = "Both market and product are required")]
    MarketProductsIsRequired,

    /// <summary>
    /// Contest key is required
    /// </summary>
    [Display(Description = "Contest is required")]
    ContestKeyIsRequired,

    /// <summary>
    /// Bet is required
    /// </summary>
    [Display(Description = "Bet is required")]
    BetIsRequired,

    /// <summary>
    /// Bets must have at least one leg/stage
    /// </summary>
    [Display(Description = "Bets must have at least 1 stage")]
    BetStageIsRequired,

    /// <summary>
    /// Bets must have a stake
    /// </summary>
    [Display(Description = "Bets must have a stake")]
    BetStakeIsRequired,

    /// <summary>
    /// Country Code must be null or Have a length of 3
    /// </summary>
    [Display(Description = "Currency Code must be null or Have a length of 3")]
    CurrencyCodeMustBeNullOrLengthOfThree,

    /// <summary>
    /// Stages must have a selection
    /// </summary>
    [Display(Description = "Stages must have a selection")]
    StageSelectionIsRequired,

    /// <summary>
    /// Odds must be specified
    /// </summary>
    [Display(Description = "Requested Odds must be specified")]
    RequestedOddsAreRequired,

    [Display(Description = "Hash is required")]
    RewardHashIsRequired,

    [Display(Description = "At least one claim must be requested")]
    ClaimsAreRequired,

    /// <summary>
    /// Contest type is required
    /// </summary>
    [Display(Description = "Contest type is required")]
    ContestTypeIsRequired,


    /// <summary>
    /// Reward claims are only sortable on certain fields
    /// </summary>
    [Display(Description = "Reward claims are only sortable on certain fields - ")]
    RewardClaimsAreOnlySortableOnCertainFields
}
