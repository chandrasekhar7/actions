namespace Npa.Accounting.Infrastructure.Lpp;

public class PaymentMessage
{
    /// <summary>
    /// The message representing the result of the request. Review the 'Response Messages' section of the developer center for reference.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The code representing the result of the request. Review the 'Response Messages' section of the developer center for reference.
    /// </summary>
    public string ResponseCode { get; set; }

    /// <summary>
    /// The resulting status of the request. Possible values are "Success", "Failure", and "Error".
    /// </summary>
    public Status Status { get; set; }
}