using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Persistence.DEPRECATED.Converters;

public class ExpirationConverter : ValueConverter<Expiration, DateTime>
{
    public ExpirationConverter() : base(
        exp => exp.ToDate(),
        d => new Expiration(d.Month, d.Year))
    {
    }
}

public class ExpirationComparer : ValueComparer<Expiration>
{
    public ExpirationComparer() : base(
        (l1,l2) => l1.ToDate() == l2.ToDate(),
        d => d.GetHashCode())
    {
    }
}