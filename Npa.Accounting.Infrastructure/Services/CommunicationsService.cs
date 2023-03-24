using System;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Communications;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;

namespace Npa.Accounting.Infrastructure.Services
{
    public class CommunicationsService : ICommunicationsService
    {
        private readonly IDbFacade facade;

        public CommunicationsService(IDbFacade fa)
        {
            facade = fa ?? throw new ArgumentNullException(nameof(fa)); 
        }

        public void SendEmail(int loanId, decimal amount)
        {
            facade.ExecProcSync("dbo.DrawApprovedCommunications", new { LoanID = loanId, Amount = amount });
        }

        public void SendPaymentConfirmSMS(int paymentID)
        {
            facade.ExecProcSync("dbo.SMSPaymentConfirm", new { PaymentID = paymentID });
        }

        public void SendInstantFundFailedSMS(int loanID)
        {
            facade.ExecProcSync("dbo.SMSInstantFundingFailed", new {loanID = loanID }); //Create a proc for this and slip it in
        }

        public void AddCustomerServiceNote(int loanId, string note, string tellerId)
        {
            facade.ExecProc("dbo.Paydini2016_NewCustomerServiceClientNoteWithLoanId", new { loanid = loanId, note = note, teller = tellerId });
        }

        public void AddAdditionalPayment(int loanId, decimal amount)
        {
            facade.ExecProc("dbo.RI_Add_Additionalpayments", new { LoanID = loanId, Amount = amount });
        }
    }
}
