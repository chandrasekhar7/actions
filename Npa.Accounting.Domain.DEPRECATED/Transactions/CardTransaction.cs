using System;
using System.Collections.Generic;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED.Customers;

namespace Npa.Accounting.Domain.DEPRECATED.Transactions;

public class CardTransaction : Entity<int>
{
    private readonly List<string> merchant702DeleteCodes = new List<string>()
    {
        "18", "19", "20", "24", "37", "44", "49", "51", "52", "82", "84", "86", "88", "89", "90", "91", "92", "94",
        "107", "193", "194"
    };
        
    //"05", "5","13","28", "51", "61", "65", "94", "96"
    private readonly List<string> merchantOtherDeleteCodes = new List<string>()
    {
        "01", "02", "03", "04", "06", "07", "12", "14", "15", "30", "34", "39", "41", "43", "54", "55", "57", "58",
        "59"
    };
    public CustomerCard Card { get; }
    public DateTime StatusDate { get; private set; }
    public DateTime ProcessDate { get; private set; }
    public ReturnMessage ReturnMessage { get; private set; }
    public Merchant MerchantId { get; }

    private CardTransaction()
    {
    }

    internal CardTransaction(CustomerCard card, Merchant merchantId, ReturnMessage returnMessage)
    {
        Card = card;
        MerchantId = merchantId;
        ReturnMessage = returnMessage;
        StatusDate = ProcessDate = DateTime.Now;
    }

    public CardTransaction(CustomerCard card, Merchant merchantId) : this(card,
        merchantId, ReturnMessage.Default)
    {
    }

    public void UpdateResult(ReturnMessage rm)
    {
        if (ReturnMessage.Status != CardReturnStatus.NotStarted)
        {
            throw new DomainLayerException("Result cannot be changed after it has been set");
        }
        ReturnMessage = rm;
        StatusDate = DateTime.Now;
        MarkCardDeletedForBadTransactions();
    }
    
    private void MarkCardDeletedForBadTransactions()
    {
        if (MerchantId.IsLPP() && merchant702DeleteCodes.Contains(ReturnMessage.Code))
        {
            Card.RemoveCard();
        }
        else if (merchantOtherDeleteCodes.Contains(ReturnMessage.Code))
        {
            Card.RemoveCard();
        }
    }

    public void Void()
    {
        if (ReturnMessage.Status != CardReturnStatus.Approve)
        {
            throw new DomainLayerException("Transaction must be Approved to void");
        }

        ReturnMessage = new ReturnMessage(CardReturnStatus.Void,
            ReturnMessage.Code, ReturnMessage.Message, ReturnMessage.RefNum);
        StatusDate = DateTime.Now;
    }
}