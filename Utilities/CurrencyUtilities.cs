using System.Globalization;
using TheProjector.Data.DTO;

namespace TheProjector.Utilities;

public class CurrencyUtilities
{
    public static bool CheckCurrencyCodeSupport(string currencyCode)
    {
        return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                        .Select(culture => new RegionInfo(culture.Name))
                        .Any(region => region.ISOCurrencySymbol == currencyCode);
    }

    public static IEnumerable<CurrencyInfo> GetSupportedCurrencyInfo()
    {
        return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => new RegionInfo(culture.Name))
            .Where(region => region.CurrencyEnglishName != String.Empty)
            .DistinctBy(region => region.ISOCurrencySymbol) // distinct
            .OrderBy(region => region.CurrencyEnglishName)
            .Select(region => new CurrencyInfo { EnglishName = region.CurrencyEnglishName, Code = region.ISOCurrencySymbol });
    }
}