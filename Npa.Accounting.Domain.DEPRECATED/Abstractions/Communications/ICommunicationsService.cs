namespace Npa.Accounting.Domain.DEPRECATED.Abstractions.Communications;

public interface ICommunicationsService
{
    void SendEmail(int loanId, decimal amount);
    public void SendPaymentConfirmSMS(int paymentID);
    public void SendInstantFundFailedSMS(int loanID);
    void AddCustomerServiceNote(int loanId, string note, string tellerId);

    void AddAdditionalPayment(int loanId, decimal amount);
}
