using System;

namespace Npa.Accounting.Domain.DEPRECATED.Draws;

public class Draw : Entity<int>
{
    public string DrawType { get; }
    public int PowerID { get; }
    public int LoanID { get; }
    public DateTime CreatedOn { get; }
    public decimal Amount { get; }
    public string IpAddress { get; }

    private Draw() {}

    public Draw(string drawType, int powerID, int loanID, decimal amount, string ipAddress)
    {
        DrawType = drawType;
        PowerID = powerID;
        LoanID = loanID;
        CreatedOn = DateTime.Now;
        Amount = amount;
        IpAddress = ipAddress;
    }
}