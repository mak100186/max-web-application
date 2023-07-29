namespace Kindred.Rewards.Core.WebApi;

public static class ApiConstants
{
    // Allow A-Z, a-z, 0-9, underscore, hyphen and dot.
    public const string ValidKeyRegEx = "^[\\w-\\.]*$";

    // Allow A-Z, a-z, 0-9, underscore, hyphen, space and dot.
    public const string ValidNameRegEx = "^[\\w-\\.\\s]*$";

    // Misc constants
    public const string Description = "Description";

    // validation errors
    public const string ErrorMsgCannotProcessInput = "There was a problem processing the input";
    public const string ErrorMsgInvalidUpdateDateFromUtc = "'UpdateDateFromUtc', if specified, must be > DateTime.MinValue";
    public const string ErrorMsgInvalidUpdateDateToUtc = "'UpdateDateToUtc', if specified, must be > DateTime.MinValue";
    public const string ErrorMsgInvalidUpdateDateToUtcDateFromUtcDependency = "'UpdatedDateToUtc' must be >= 'UpdateDateFromUtc'";
}
