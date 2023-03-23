using System;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Application.CardTransactions;

public class CardTransactionViewModel
{
    public int Id { get; init; }
    public int LoanId { get; init; }
    public TransactionType TransactionType { get; init; }
    public decimal Amount { get; set; }
    public DateTime CreatedOn { get; init; }
    public string Teller { get; init; } = default!;
    public int CardToken { get; init; }
    public DateTime StatusDate { get; init; }
    public string ReturnCode { get; init; } = default!;
    public int MerchantId { get; init; }
    public string? ReturnMessage { get; init; }
    public string? RefNum { get; init; }
    public string? LastFour { get; init; }

    private CardTransactionViewModel()
    {
    }

    public CardTransactionViewModel(Transaction trans)
    {
        Amount = trans.Amount;
        Id = trans.Id;
        LoanId = trans.LoanId;
        TransactionType = trans.TransactionType;
        CreatedOn = trans.CreatedOn;
        Teller = trans.Teller.ToString();
        CardToken = trans.CardTransaction.Card.CardToken;
        StatusDate = trans.CreatedOn;
        ReturnCode = GetReturnCode(trans.CardTransaction.ReturnMessage.Status);
        MerchantId = trans.CardTransaction.MerchantId;
        RefNum = trans.CardTransaction.ReturnMessage.RefNum;
        ReturnMessage = trans.CardTransaction.ReturnMessage.Message;
    }

    private static string GetReturnCode(CardReturnStatus c) => c switch
    {
        CardReturnStatus.Deny => "D",
        CardReturnStatus.Error => "E",
        CardReturnStatus.Approve => "A",
        CardReturnStatus.Void => "V",
        CardReturnStatus.NotStarted => "N",
        _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
    };
}