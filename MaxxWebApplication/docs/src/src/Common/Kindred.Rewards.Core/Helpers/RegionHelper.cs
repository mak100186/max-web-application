using System.Globalization;

namespace Kindred.Rewards.Core.Helpers;

public static class RegionHelper
{

    /// <summary>
    /// Gets the list of countries based on ISO 3166-1
    /// </summary>
    /// <returns>Returns the list of countries based on ISO 3166-1</returns>
    public static IReadOnlyCollection<RegionInfo> GetCountriesByIso3166()
    {
        List<RegionInfo> allRegions = new();
        foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            try
            {
                RegionInfo country = new(culture.LCID);
                if (allRegions.All(p => p.Name != country.Name))
                {
                    allRegions.Add(country);
                }
            }
            catch
            {
                //ignored. this occurs when unknown cultures are found on the system.
                //see https://social.msdn.microsoft.com/Forums/ie/en-US/671bc463-932d-4a9e-bba1-3e5898b9100d/culture-4096-0x1000-is-an-invalid-culture-identifier-culturenotfoundexception?forum=csharpgeneral
            }
        }
        return allRegions.OrderBy(p => p.EnglishName).ToList();
    }

    public static bool IsIso3Valid(string code)
    {
        return !string.IsNullOrWhiteSpace(code)
               && GetCountriesByIso3166().Any(x => x.ThreeLetterISORegionName.Equals(code, StringComparison.InvariantCultureIgnoreCase));
    }

    public static bool IsIso2Valid(string code)
    {
        return !string.IsNullOrWhiteSpace(code)
               && GetCountriesByIso3166().Any(x => x.TwoLetterISORegionName.Equals(code, StringComparison.InvariantCultureIgnoreCase));
    }
}
