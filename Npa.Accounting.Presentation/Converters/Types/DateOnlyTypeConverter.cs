using System;

namespace Npa.Accounting.Presentation.Converters.Types;

public class DateOnlyTypeConverter : StringTypeConverterBase<DateOnly>
{
    protected override DateOnly Parse(string s) => DateOnly.ParseExact(s, AcceptedFormats.DateFormats);

    protected override string ToIsoString(DateOnly source) => source.ToString("O");
}