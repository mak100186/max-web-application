using System.ComponentModel.DataAnnotations;

namespace Kindred.Rewards.Core.WebApi.Validation.Enums;

/// <summary>
/// Error codes
/// </summary>
public enum ErrorCodes
{
    /// <summary>
    /// The cannot process input.
    /// </summary>
    [Display(Description = ApiConstants.ErrorMsgCannotProcessInput)]
    CannotProcessInput = 100,

    /// <summary>
    /// The update date from UTC invalid.
    /// </summary>
    [Display(Description = ApiConstants.ErrorMsgInvalidUpdateDateFromUtc)]
    UpdateDateFromUtcInvalid = 101,

    /// <summary>
    /// The update date to UTC invalid.
    /// </summary>
    [Display(Description = ApiConstants.ErrorMsgInvalidUpdateDateToUtc)]
    UpdateDateToUtcInvalid = 102,

    /// <summary>
    /// The update date from to UTC dependency invalid.
    /// </summary>
    [Display(Description = ApiConstants.ErrorMsgInvalidUpdateDateToUtcDateFromUtcDependency)]
    UpdateDateFromToUtcDependencyInvalid = 103,

    /// <summary>
    /// Offset must be greater than or equal to 0 if provided
    /// </summary>
    [Display(Description = "Offset must be greater than or equal to 0 if provided")]
    OffsetMustBeGreaterThanOrEqualToZeroIfProvided = 104,

    /// <summary>
    /// Limit must be greater than or equal to 0 if provided
    /// </summary>
    [Display(Description = "Limit must be greater than or equal to 0 if provided")]
    LimitMustBeGreaterThanOrEqualToZeroIfProvided = 105
}
