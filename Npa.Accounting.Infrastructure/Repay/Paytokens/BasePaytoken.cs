namespace Npa.Accounting.Infrastructure.Repay.Paytokens;

public abstract class BasePaytoken
{
    public CheckoutFormKey CheckoutFormKey { get; protected set; }
    public abstract void SetPayToken(PaytokenResponse r);
}