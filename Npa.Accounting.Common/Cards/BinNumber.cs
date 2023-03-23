using System;
using System.Text.RegularExpressions;

namespace Npa.Accounting.Common.Cards
{
    public record BinNumber
    {
        private static readonly Regex regex = new Regex(@"^\d{6}$", RegexOptions.Compiled | RegexOptions.Singleline);

        public BinNumber(string Value)
        {
            var value = string.IsNullOrWhiteSpace(Value) ? throw new ArgumentNullException(nameof(Value)) : Value.Trim().Replace("*", "");
            if (!IsValid(value))
            {
                throw new FormatException($"BinNumber '{Value}' is in an incorrect format.");
            }

            this.Value = value;
        }
        public string Value { get; }
        
        public override string ToString() => Value;

        private static bool IsValid(string value) => regex.IsMatch(value);
    }
}