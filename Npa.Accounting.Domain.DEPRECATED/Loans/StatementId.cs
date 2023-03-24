using System;
using System.Globalization;

namespace Npa.Accounting.Domain.DEPRECATED.Loans;

public record StatementId
{
    public int LoanId { get; }
    public DateOnly StatementDate { get; }

    public StatementId(string stmtId)
    {
        var split = stmtId.Split('-');
        if (split.Length != 2 || !int.TryParse(split[0], out var loanId) || !DateOnly.TryParseExact(split[1],"yyyyMMdd",CultureInfo.InvariantCulture, DateTimeStyles.None,out var date))
        {
            throw new ArgumentException("Invalid statement Id");
        }

        LoanId = loanId;
        StatementDate = date;
    }
    public StatementId(int loanId, DateOnly stmtDate)
    {

        LoanId = loanId;
        StatementDate = stmtDate;
    }

    public override string ToString() => $"{LoanId}-{StatementDate}";
}