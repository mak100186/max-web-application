namespace Kindred.Rewards.Core.WebApi.Payloads;

public class PlatformRestrictionsApiModel
{
    public string CountryCode { get; set; }
    public string Jurisdiction { get; set; }
    public string State { get; set; }
    public string Brand { get; set; }
    public string CurrencyCode { get; set; }
    public override int GetHashCode()
    {
        return HashCode.Combine(CountryCode, Jurisdiction, Brand, State, CurrencyCode);
    }
}
