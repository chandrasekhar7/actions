using System;
using System.Text.RegularExpressions;

namespace Npa.Accounting.Common.Bank
{
    public record BankAccount
    {
        private static readonly Regex routingRegex = new Regex(@"^\d{9}$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex accountRegex = new Regex(@"^\d+$", RegexOptions.Compiled | RegexOptions.Singleline);

        public string RoutingNumber { get; }
        public string AccountNumber { get; }
        
        public BankAccount(string routing, string account)
        {
            if (!IsValidRoutingNumber(routing))
            {
                throw new FormatException("Routing number must be 9 digits");
            }
            RoutingNumber = routing;

            if (!IsValidAccountNumber(account))
            {
                throw new FormatException("Account number must be numeric");
            }

            AccountNumber = account;
        }
        
        private static bool IsValidRoutingNumber(string value) => routingRegex.IsMatch(value);
        private static bool IsValidAccountNumber(string value) => accountRegex.IsMatch(value);
    }
}