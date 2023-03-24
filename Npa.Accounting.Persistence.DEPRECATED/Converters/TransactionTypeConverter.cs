using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npa.Accounting.Common.Transactions;

namespace Npa.Accounting.Persistence.DEPRECATED.Converters;

public class TransactionTypeConverter : ValueConverter<TransactionType, byte>
{
    public TransactionTypeConverter() : base(
        m => (byte)(int)m,
        i => (TransactionType)i)
    {
    }
}

public class TransactionTypeComparer : ValueComparer<TransactionType>
{
    public TransactionTypeComparer() : base(
        (l1,l2) => l1 == l2,
        d => d.GetHashCode())
    {
    }
}