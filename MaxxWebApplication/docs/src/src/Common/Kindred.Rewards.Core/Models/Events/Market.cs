namespace Kindred.Rewards.Core.Models.Events;

public class Market
{
    public string MarketTypeKey { get; set; }

    public int Denominator { get; set; }

    public decimal Numerator { get; set; }

    public List<string> Products { get; set; }
}
