using System.ComponentModel.DataAnnotations;

using FluentValidation;

using Kindred.Infrastructure.Core.Extensions.Extensions;

namespace Kindred.Rewards.Core.WebApi.Validation;

public class ValidatorBase<T> : AbstractValidator<T>
{
    protected string GenerateErrorMessage<TEnum>(TEnum errorCode)
    {
        var code = (int)Enum.Parse(typeof(TEnum), errorCode.ToString());
        var description = errorCode.GetAttributeValue<DisplayAttribute>(ApiConstants.Description);

        return $"{code}|{description}";
    }
}
