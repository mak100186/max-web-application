using System.ComponentModel.DataAnnotations;

namespace Kindred.Rewards.Core.WebApi.Validation.Enums;

public enum PromotionTemplateErrorCodes
{
    /// <summary>
    /// Promotion template key should be valid
    /// </summary>
    [Display(Description = "Promotion template key should not contain any special character")]
    TemplateKeyShouldNotContainSpecialChar = 401,

    /// <summary>
    /// Promotion template key cannot have leading or trailing dot character
    /// </summary>
    [Display(Description = "Promotion template key should not have leading or trailing dot character")]
    TemplateKeyShouldNotContainLeadingOrTrailingDot = 402,

    /// <summary>
    /// Promotion template description should not be longer than 300 characters
    /// </summary>
    [Display(Description = "Promotion template description should not be longer than 300 characters")]
    TemplateDescriptionShouldNotBeLongerThan300Characters = 403,

    /// <summary>
    /// Promotion template title should not be longer than 300 characters
    /// </summary>
    [Display(Description = "Promotion template title should not be longer than 300 characters")]
    TemplateTitleShouldNotBeLongerThan300Characters = 404
}
