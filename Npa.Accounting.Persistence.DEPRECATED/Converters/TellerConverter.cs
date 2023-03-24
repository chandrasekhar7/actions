using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npa.Accounting.Common;

namespace Npa.Accounting.Persistence.DEPRECATED.Converters;

public class TellerConverter : ValueConverter<Teller, string>
{
    public TellerConverter() : base(
        teller => teller.ToString(),
        str => new Teller(str))
    {
    }
}

public class TellerComparer : ValueComparer<Teller>
{
    public TellerComparer() : base(
        (l1,l2) => l1.ToString() == l2.ToString(),
        d => d.Value.GetHashCode())
    {
    }
}