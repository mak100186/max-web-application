namespace Kindred.Rewards.Plugin.Discovery.Models;

public class CountriesResponse
{
    public ICollection<CountryResponse> Countries { get; set; } = new List<CountryResponse>();
}

public class CountryResponse
{
    public string Name { get; set; }
    public string Code { get; set; }
}
