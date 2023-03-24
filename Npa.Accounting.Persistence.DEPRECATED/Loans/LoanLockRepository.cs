using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Loans;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;

namespace Npa.Accounting.Persistence.DEPRECATED.Loans;

public class LoanLockRepository : ILoanLockRepository
{
    private readonly IDbFacade facade;
    
    private List<LoanLock> locked = new();
    
    public LoanLockRepository(IDbFacade facade)
    {
        this.facade = facade;
    }

    public async Task<bool> TryLock(LoanLock loan)
    {
        var attempt = await facade.ExecSingleProc<LockAttempt>("loan.LockLoan", loan);
        if (attempt.IsSuccessfull)
        {
            locked.Add(loan);
            return true;
        }
        return false;
    }

    public async Task<bool> TryUnlock(LoanLock loan)
    {
        if (locked.Contains(loan))
        {
            await facade.ExecProc("loan.UnlockLoan", loan);
            locked.Remove(loan);
            return true;
        }
        return false;
    }
    
    private void ReleaseUnmanagedResources()
    {
        foreach (var loan in locked)
        {
            facade.ExecProcSync("loan.UnlockLoan", loan);
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~LoanLockRepository()
    {
        ReleaseUnmanagedResources();
    }
}