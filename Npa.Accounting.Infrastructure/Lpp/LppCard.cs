using System;
using Npa.Accounting.Infrastructure.Crypto;

namespace Npa.Accounting.Infrastructure.Lpp;

internal class LppCard
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address1 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Cvv { get; set; }
    public string CardNumber { get; set; }
    public string ExpMonth { get; set; }
    public string ExpYear { get; set; }
    public string InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string UDF01 { get; set; }
    public string UDF02 { get; set; }

    public static LppCard From(CryptoCard c, decimal amount) => new LppCard
    {
        FirstName = c.FirstName,
        LastName = c.LastName,
        Address1 = c.Address1,
        City = c.City,
        State = c.State,
        Zip = c.Zip,
        Cvv = c.Cvv,
        CardNumber = c.CardNumber,
        ExpMonth = c.ExpirationDate.Substring(0, 2),
        ExpYear = c.ExpirationDate.Substring(2, 2),
        Amount = amount,
        UDF01 = $"{c.FirstName} {c.LastName}",
        UDF02 = DateTime.Now.ToString()
    };
}