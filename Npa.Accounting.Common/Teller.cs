using System;
using System.Collections.Generic;

namespace Npa.Accounting.Common
{
    public record Teller
    {
        public string Value { get; }
        public Teller(string initials)
        {
            if (initials.Length != 3)
            {
                throw new ArgumentException($"{nameof(initials)} must be 3 characters");
            }

            Value = initials;
        }

        public override string ToString() => Value;

        private readonly List<string> AdditionalPaymentTellers = new List<string>()
        {
            "ILM", "APP", "EAP", "YAK"
        };

        public bool isAdditionalPaymentTellers(string teller)
        {
            return AdditionalPaymentTellers.Contains(teller);
        }
    }

  
}