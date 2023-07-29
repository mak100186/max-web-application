using Kindred.Infrastructure.Core.Extensions.Extensions;

namespace Kindred.Rewards.Core.Extensions;

public static class DecimalExtensions
{
    public static decimal RoundToDecimalPlaces(this decimal value, string currencyCode, int fallbackDecimalPlaces = 2)
    {
        var decimalPlaces = fallbackDecimalPlaces;
        if (currencyCode != null)
        {
            decimalPlaces = CurrencyExtensions.GetDecimalDigits(currencyCode);
        }

        return Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
    }
}
