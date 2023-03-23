using System.Globalization;
using System.Linq;

namespace Npa.Accounting.Presentation.Converters;

public static class AcceptedFormats
{
    private static readonly CultureInfo culture = new CultureInfo("en-US");
    public static readonly string[] DateFormats = new[] { "M-d-yyyy", "MM-dd-yyyy", "yyyyMMdd", "MMddyyyy", "MM/dd/yyyy", "MM/dd/yy", "yyyy-MM-dd" }
        .Union(culture.DateTimeFormat.GetAllDateTimePatterns()).ToArray();
}