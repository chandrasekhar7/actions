using Npa.Accounting.Common.Addresses;
using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Domain.Cards;

public class CardInfo
{
    public bool CanDisburse { get; }
    public BinNumber Bin { get; }
    public Address BillingAddress { get; }
    public string BankName { get; }
    public string Brand { get; }
    public string CardType { get; }

    private CardInfo()
    {
        
    }

    public CardInfo(BinNumber bin, Address billingAddress, string brand, string cardType, string bankName, bool canDisburse)
    {
        Bin = bin;
        BillingAddress = billingAddress;
        BankName = bankName;
        Brand = brand;
        CardType = cardType;
        CanDisburse = canDisburse;
    }
}