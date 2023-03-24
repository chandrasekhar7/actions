using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Persistence.DEPRECATED.Converters;

enum StatusConversion
{
    D, E, A, V, N
}

public class CardStatusConverter : ValueConverter<CardReturnStatus, string>
{
    public CardStatusConverter() : base(
        s => s.ToString().First().ToString(),
        d => (CardReturnStatus)(int)Enum.Parse(typeof(StatusConversion), d))
    {
    }
}

public class CardStatusComparer : ValueComparer<CardReturnStatus>
{
    public CardStatusComparer() : base(
        (l1,l2) => l1 == l2,
        d => d.GetHashCode())
    {
    }
}