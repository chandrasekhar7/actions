namespace Npa.Accounting.Infrastructure.Repay;

public class RepayOptions
{
    public static string Key => "Repay";
    public string Uri { get; set; }
    public string Auth { get; set; }
    public CheckoutForm CheckoutForm { get; set; }
    public string Source { get; set; } = "DirectAPI";
    public string PaymentChannel { get; set; } = "web";
    public string ChannelUser { get; set; } = "btefft@repay.io";
}

public class CheckoutForm
{
    public string CardAuth { get; set; }
    public string TokenPayment { get; set; }
}