using FluentValidation;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Validation;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Plugin.Reward.Models;

public abstract class RewardRequestValidatorBase<T> : ValidatorBase<T>
    where T : RewardRequest
{

    protected void AddRewardValidationRules()
    {
        RuleFor(p => p.RewardType.ToEnumSafe<RewardType>()).NotNull().WithMessage(
        GenerateErrorMessage(RewardErrorCodes.RewardTypeIsInvalid));

        RuleFor(p => p.Restrictions).NotEmpty()
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.RestrictionsIsRequired));

        RuleFor(p => p.RewardParameters).NotEmpty().WithMessage(
            GenerateErrorMessage(RewardErrorCodes.RewardParametersIsRequired));

        When(x => string.Equals(x.RewardType, RewardType.Freebet.ToString(), StringComparison.InvariantCultureIgnoreCase), () =>
        {
            RuleFor(y => y.RewardParameters)
                .Must(rp => rp.GetType() == typeof(FreeBetParametersApiModel))
                .WithMessage(GenerateErrorMessage(RewardErrorCodes.RewardParameterTypeMustMatchRewardType));
        });

        When(x => string.Equals(x.RewardType, RewardType.Profitboost.ToString(), StringComparison.InvariantCultureIgnoreCase), () =>
        {
            RuleFor(y => y.RewardParameters)
                .Must(rp => rp.GetType() == typeof(ProfitBoostParametersApiModel))
                .WithMessage(GenerateErrorMessage(RewardErrorCodes.RewardParameterTypeMustMatchRewardType));
        });

        When(x => string.Equals(x.RewardType, RewardType.Uniboost.ToString(), StringComparison.InvariantCultureIgnoreCase), () =>
        {
            RuleFor(y => y.RewardParameters)
                .Must(rp => rp.GetType() == typeof(UniBoostParametersApiModel))
                .WithMessage(GenerateErrorMessage(RewardErrorCodes.RewardParameterTypeMustMatchRewardType));
        });

        When(x => string.Equals(x.RewardType, RewardType.UniboostReload.ToString(), StringComparison.InvariantCultureIgnoreCase), () =>
        {
            RuleFor(y => y.RewardParameters)
                .Must(rp => rp.GetType() == typeof(UniBoostReloadParametersApiModel))
                .WithMessage(GenerateErrorMessage(RewardErrorCodes.RewardParameterTypeMustMatchRewardType));
        });

        RuleFor(x => x.RewardParameters)
            .SetInheritanceValidator(v =>
            {
                v.Add(new FreebetRewardParameterValidator());
                v.Add(new UniboostRewardParameterValidator());
                v.Add(new UniboostReloadRewardParameterValidator());
                v.Add(new ProfitBoostRewardParameterValidator());
            });

        RuleFor(x => x.DomainRestriction).SetValidator(new DomainRestrictionApiModelValidator());
        When(
            p => p.Restrictions != null,
            () =>
                {
                    When(
                        r => r.Restrictions.StartDateTime.HasValue,
                        () => RuleFor(p => p.Restrictions.StartDateTime).LessThan(p => DateTime.MaxValue)
                            .WithMessage(
                                GenerateErrorMessage(RewardErrorCodes.StartDateShouldBeLessThanMaxDate)));

                    When(
                        r => r.Restrictions.ExpiryDateTime.HasValue,
                        () => RuleFor(p => p.Restrictions.ExpiryDateTime).GreaterThan(p => DateTime.UtcNow)
                            .WithMessage(
                                GenerateErrorMessage(
                                    RewardErrorCodes.ExpiryDateShouldBeGreaterThanUtcNow)));

                    When(
                        r => r.Restrictions.StartDateTime.HasValue && r.Restrictions.ExpiryDateTime.HasValue,
                        () =>
                            {
                                RuleFor(p => p.Restrictions.StartDateTime)
                                    .LessThan(p => p.Restrictions.ExpiryDateTime).WithMessage(
                                        GenerateErrorMessage(
                                            RewardErrorCodes.StartDateShouldBeLessThanExpiryDate));

                                RuleFor(p => p.Restrictions.ExpiryDateTime)
                                    .GreaterThan(p => p.Restrictions.StartDateTime).WithMessage(
                                        GenerateErrorMessage(
                                            RewardErrorCodes.ExpiryDateShouldBeGreaterThanStartDate));
                            });

                    RuleFor(p => CronService.IsValidCron(p.Restrictions.ClaimInterval ?? string.Empty))
                        .Equal(true).WithMessage(
                            GenerateErrorMessage(RewardErrorCodes.ClaimIntervalExpressionIsNotValid));

                    RuleFor(p => CronService.IsValidCron(p.Restrictions.ClaimAllowedPeriod ?? string.Empty))
                        .Equal(true).WithMessage(
                            GenerateErrorMessage(RewardErrorCodes.ClaimAllowedPeriodExpressionIsNotValid));

                    When(
                        r => r.Restrictions.ClaimsPerInterval.HasValue,
                        () => RuleFor(r => r.Restrictions.ClaimsPerInterval).GreaterThan(0).WithMessage(
                            GenerateErrorMessage(RewardErrorCodes.ClaimPerIntervalCannotBeZero)));


                    When(
                        r => !string.IsNullOrWhiteSpace(r.Restrictions.TimezoneId),
                        () =>
                            {
                                RuleFor(r => r.Restrictions.ClaimAllowedPeriod).NotEmpty().WithMessage(
                                    GenerateErrorMessage(
                                        RewardErrorCodes.ClaimAllowedPeriodIsRequiredWhenTimezoneIsGiven));

                                RuleFor(r => r.Restrictions.TimezoneId).Must(
                                    id =>
                                        {
                                            try
                                            {
                                                TimeZoneInfo.FindSystemTimeZoneById(id);
                                            }
                                            catch (Exception)
                                            {
                                                return false;
                                            }

                                            return true;
                                        }).WithMessage(
                                    GenerateErrorMessage(RewardErrorCodes.TimezoneIdShouldBeValid));
                            });
                });

        //this.RuleFor(p => p)
        //    .Must(p => p.SubPurpose.IsValid(p.Purpose))
        //    .WithMessage("Sub purpose is invalid");

        RuleFor(p => p)
            .Must(p => RegionHelper.IsIso3Valid(p.CountryCode))
            .WithMessage("Country code is invalid");

        RuleFor(p => p.State)
            .MaximumLength(50)
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.StateHasMaximumLengthOfFiftyCharacters));

        RuleFor(p => p.CurrencyCode)
            .MaximumLength(20)
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.CurrencyHasAMaximumLengthOfTwenty));
    }
}
