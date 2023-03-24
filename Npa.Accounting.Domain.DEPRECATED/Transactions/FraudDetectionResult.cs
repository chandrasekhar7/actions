
namespace Npa.Accounting.Domain.DEPRECATED.Transactions
{
    public class FraudDetectionResult
    {
        public FraudDetectionResult(bool isEligible, string? loanLockNote, bool cCBlock, string? errorMessage, bool runDeny)
        {
            LoanLockNote = loanLockNote ?? "";
            ErrorMessage = errorMessage ?? "";
            IsEligible = isEligible;
            CCBlock = cCBlock;
            RunDeny = runDeny;
        }

        public FraudDetectionResult()
        {
        }

        public bool IsEligible { get; set; } = true;
        public string? LoanLockNote { get; set; }
        public bool CCBlock { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public bool RunDeny { get; set; } = false;
        public bool AlreadyLocked { get; set; } = false;

    }
}