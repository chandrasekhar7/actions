using System;
using Npa.Accounting.Domain.DEPRECATED.Customers;

namespace Npa.Accounting.Domain.DEPRECATED.Transactions;

public record Merchant
{
    public int Id { get; }

    private Merchant()
    {
    }

    public Merchant(int id) => this.Id = Merchant.IsValid(id) ? id : throw new ArgumentOutOfRangeException("Invalid merchant id");

    public Merchant(Location location) : this((int) location)
    {
    }

    public static implicit operator int(Merchant m) => m.Id;

    private static bool IsValid(int id) => id is 700 or 701 or 702 or 703 or 704 or 705;
    
    public bool IsLPP() => Id is 702 or 703 or 704 or 705;

    public bool IsRepay() => Id is 700 or 701;

    protected Merchant(Merchant original) => Id = original.Id;
}