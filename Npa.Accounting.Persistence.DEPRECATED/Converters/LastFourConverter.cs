using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Persistence.DEPRECATED.Converters;

public class LastFourConverter : ValueConverter<LastFour, string>
{
    public LastFourConverter() : base(
        lastFour => lastFour.ToString(),
        str => new LastFour(str))
    {
    }
}

public class LastFourComparer : ValueComparer<LastFour>
{
    public LastFourComparer() : base(
        (l1,l2) => l1.ToString() == l2.ToString(),
        d => d.Value.GetHashCode())
    {
    }
}