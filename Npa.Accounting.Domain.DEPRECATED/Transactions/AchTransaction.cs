using System;
using Npa.Accounting.Common.Transactions;

namespace Npa.Accounting.Domain.DEPRECATED.Transactions;

public class AchTransaction : Entity<int>
{
    public DateTime? StatusDate { get; init; }
    public string? ReturnCode { get; init; }
    public string? ReturnMessage { get; init; }
    public bool? Success { get; init; }
    public bool? IsCdr { get; init; }
    public string AccountNumber { get; set; }
    public string RoutingNumber { get; init; }
    public string? BatchNum { get; set; }
    public string KeyTable { get; init; }
    public BankAccountType AccountType { get; init; }
}