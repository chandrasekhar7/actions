using System;
using System.Text.RegularExpressions;

namespace Npa.Accounting.Common.Cards
{
    public record LastFour
    {
        private static readonly Regex regex = new Regex(@"^\d{4}$", RegexOptions.Compiled | RegexOptions.Singleline);
        
        private LastFour() {}

        public LastFour(string Value)
        {
            var value = string.IsNullOrWhiteSpace(Value) ? throw new ArgumentNullException(nameof(Value)) : Value.Trim().Replace("*", "");
            if (!IsValid(value))
            {
                throw new FormatException($"CardNumber '{Value}' is in an incorrect format.");
            }

            this.Value = value;
        }
        public string Value { get; }
        
        public override string ToString() => Value;

        private static bool IsValid(string value) => regex.IsMatch(value);
    }
}