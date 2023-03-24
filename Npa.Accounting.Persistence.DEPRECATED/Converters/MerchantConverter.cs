using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Persistence.DEPRECATED.Converters;

public class MerchantConverter : ValueConverter<Merchant, int>
{
    public MerchantConverter() : base(
        m => m.Id,
        i => new Merchant(i))
    {
    }
}

public class MerchantComparer : ValueComparer<Merchant>
{
    public MerchantComparer() : base(
        (l1,l2) => l1.ToString() == l2.ToString(),
        d => d.Id.GetHashCode())
    {
    }
}