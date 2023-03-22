
using System.Globalization;
namespace TheProjector.Extensions;

public static class DecimalExtensions
{
    public static string Shorthand(this decimal value)
    {
        if (value < 1000)
        {
            return value.ToString("#,0");
        }

        string[] suffixes = { "", "K", "M", "B", "T" }; // add more suffixes as needed
        int iSuffix = 0;
        while (value > 1000)
        {
            iSuffix++;
            value /= 1000;
        }
        return $"{value.ToString("#.##", CultureInfo.CurrentCulture)}{suffixes[iSuffix]}";
    }

    public static string Localized(this decimal value)
    {
        return $"{value.ToString("#,##.##", CultureInfo.CurrentCulture)}";
    }
}