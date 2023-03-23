using System;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Loans;

namespace Npa.Accounting.Domain.DEPRECATED.Abstractions.Loans;

public interface ILoanLockRepository : IDisposable
{
    Task<bool> TryLock(LoanLock loan);

    Task<bool> TryUnlock(LoanLock loan);
}