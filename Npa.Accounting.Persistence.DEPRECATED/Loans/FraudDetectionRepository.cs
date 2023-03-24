using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Loans;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Application;
using System.Threading.Tasks;
using Npa.Accounting.Common;
using System.Linq;

namespace Npa.Accounting.Persistence.DEPRECATED.Loans
{
    public class FraudDetectionRepository: IFraudDetectionRepository
    {
        private readonly IDbFacade facade;

        static string[] RepayFraudReturnMessages = {
            "pick up card, special condition (fraud account)",
            "stolen card, pick up (fraud account)",
            "lost card, pick up (fraud account)",
            "pick up card (no fraud)",
            "no checking account",
            "no credit account",
            "no account" };

        static string[] LppFraudReturnMessages = {
            "pick up card, special condition",
            "hold-call or pick up card",
            "no checking account",
            "no credit account",
            "no check account",
            "suspected fraud",
            "closed account",
            "pick up card",
            "stolen card",
            "no account",
            "lost card" };

        public FraudDetectionRepository(IDbFacade facade)
        {
            this.facade = facade;
        }

        public async Task<bool> FraudCheck(FraudDetection fraudDetection)
        {
            var fraudCheck = await facade.ExecSingleProc<FraudDetectionResult>("dbo.FraudDetection", fraudDetection);

            if (fraudCheck.AlreadyLocked)
            {
                throw new ApplicationLayerException(fraudCheck.ErrorMessage);
            }

            if (!fraudCheck.IsEligible )
            {
                await ExecFraudLock(fraudCheck.LoanLockNote ?? "", fraudCheck.ErrorMessage ?? "", fraudDetection.PowerID);
            }

            return fraudCheck.IsEligible;
        }

        public async Task<bool> ExecFraudLock(string loanLockNote, string errorMessage, int powerID)
        {
            await facade.ExecSingleProc<bool>("dbo.DrawFraudLoanLockCCDebitLock", new { PowerID = powerID, LoanLockNote = loanLockNote, CCBlock = true });
            throw new ApplicationLayerException(errorMessage);
        }
        public bool LookupFraudReturnCode(Transaction transaction, Teller teller)
        {
            if (teller.Value != "ILM") return false;

            if(transaction.CardTransaction == null)
            {
                throw new ApplicationLayerException("Card Transaction is null");
            }

            if (transaction.CardTransaction.MerchantId.IsRepay())
            {
                return RepayFraudReturnMessages.Any(x => transaction.CardTransaction.ReturnMessage.Message.ToLower().Contains(x));
            }
            else
            {
                return LppFraudReturnMessages.Any(x => transaction.CardTransaction.ReturnMessage.Message.ToLower().Contains(x));
            }
        }
    }
}
