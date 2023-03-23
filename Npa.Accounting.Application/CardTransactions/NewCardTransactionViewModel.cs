

namespace Npa.Accounting.Application.CardTransactions
{
    public class NewCardTransactionViewModel : NewTransactionViewModel
    {
        /// <summary>
        /// The card for the given transaction
        /// </summary>
        public CardViewModel Card { get; init; }
    }
}