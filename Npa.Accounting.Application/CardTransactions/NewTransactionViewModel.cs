
using Npa.Accounting.Common.Transactions;

namespace Npa.Accounting.Application.CardTransactions
{
    public class NewTransactionViewModel
    {
        /// <summary>
        /// Loan Identifier
        /// </summary>
        /// <example>1500000</example>
        public int LoanId { get; init; }
        
        /// <summary>
        /// The statement Id which is the OrigDueDate expressed in yyyyMMdd format
        /// </summary>
        /// <example>20220328</example>
        public string? StatementId { get; init; }

        /// <summary>
        /// Amount of the transaction
        /// </summary>
        /// <example>100</example>
        public decimal Amount { get; init; }

        /// <summary>
        /// The type of transaction to be committed
        /// </summary>
        /// <example>Debit</example>
        public TransactionType TransactionType { get; init; }

        /// <summary>
        /// The payment ID of the transaction to rescind
        /// </summary>
        /// <example></example>
        public int? rescindPaymentID { get; init; }
    }
}