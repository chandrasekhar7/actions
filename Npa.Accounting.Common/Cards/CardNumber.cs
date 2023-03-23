using System;
using System.Text.RegularExpressions;

namespace Npa.Accounting.Common.Cards
{
    public record CardNumber
    {
        private static readonly Regex regex = new Regex(@"^\d{16}$", RegexOptions.Compiled | RegexOptions.Singleline);

        public CardNumber(string Value)
        {
            var value = string.IsNullOrWhiteSpace(Value) ? throw new ArgumentNullException(nameof(Value)) : Value.Trim();
            if (!IsValid(value))
            {
                throw new FormatException($"CardNumber '{Value}' is in an incorrect format.");
            }

            this.Value = value;
        }

        public void Deconstruct(out string Value) => Value = this.Value;

        public string Value { get; }
        
        public static implicit operator string(CardNumber n) => n.ToString();

        private static bool IsValid(string value) => regex.IsMatch(value);

        public LastFour GetLastFour() => new LastFour(Value.Substring(13, 4));
        
        public BinNumber GetBinNumber() => new BinNumber(Value.Substring(0, 6));

        public override string ToString() => Value;
    }
}