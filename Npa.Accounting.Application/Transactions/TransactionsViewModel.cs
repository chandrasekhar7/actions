using System.Collections.Generic;
using Npa.Accounting.Application.AchTransactions;
using Npa.Accounting.Application.CardTransactions;

namespace Npa.Accounting.Application.Transactions;

public class TransactionsViewModel
{
    public IReadOnlyList<CardTransactionViewModel> CardTransactions { get; set; }
    public IReadOnlyList<AchTransactionViewModel> AchTransactions { get; set; }
}