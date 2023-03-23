using Npa.Accounting.Domain.DEPRECATED.Transactions;
using System.Threading.Tasks;
using Npa.Accounting.Common;

namespace Npa.Accounting.Domain.DEPRECATED.Abstractions.Loans
{
    public interface IFraudDetectionRepository
    {
        Task<bool> ExecFraudLock(string loanLockNote, string errorMessage, int powerID);
        bool LookupFraudReturnCode(Transaction transaction, Teller teller);
        Task<bool> FraudCheck(FraudDetection fraudDetection);
    }
} 
