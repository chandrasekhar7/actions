using System;
using System.Text.RegularExpressions;
using Npa.Accounting.Common.Text;

namespace Npa.Accounting.Common.Addresses
{
    public record ZipCode
    {
        private static readonly Regex regex = new Regex(@"^\d{5}(\-|\s*)?(\d{4})?$", RegexOptions.Compiled | RegexOptions.Singleline);

        public ZipCode(string Value)
        {
            var value = string.IsNullOrWhiteSpace(Value) ? throw new ArgumentNullException(nameof(Value)) : Value.Trim();
            if (!IsValid(value))
            {
                throw new FormatException($"Zip code '{Value}' is in an incorrect format.");
            }

            this.Value = value.StripNonDigits();
        }

        public string Value { get; init; }

        public static explicit operator ZipCode(string value) => new ZipCode(value);

        public static implicit operator string(ZipCode zipCode) => zipCode.Value;

        public override string ToString() => Value.Length == 5 ? Value : Value.Insert(5, "-");

        public string ToFiveDigit() => Value.Substring(0, 5);

        private static bool IsValid(string value) => regex.IsMatch(value);
    }
}