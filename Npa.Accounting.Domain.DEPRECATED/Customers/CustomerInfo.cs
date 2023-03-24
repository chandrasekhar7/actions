namespace Npa.Accounting.Domain.DEPRECATED.Customers;

public record CustomerInfo()
{
    public bool IsPraLimit { get; init; }
    public bool IsMilitary { get; init; }
    public bool IsLoanLocked { get; init; }
    public int Location { get; init; }
    public bool CanFund() => !(this.IsLoanLocked || this.IsMilitary || this.IsPraLimit);
}