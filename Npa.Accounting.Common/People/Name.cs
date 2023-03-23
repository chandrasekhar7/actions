using System;
using Npa.Accounting.Common.Text;

namespace Npa.Accounting.Common.People
{
    public record Name
    {
        public string First { get; }
        public string MiddleInitial { get; }
        public string Last { get; }
        
        public Name(string firstName, string lastName)
        {
            var f = firstName?.Trim();
            var l = lastName?.Trim();
            if (!IsValid(f))
            {
                throw new ArgumentException("First name is invalid");
            }
            if (!IsValid(l))
            {
                throw new ArgumentException("Last name is invalid");
            }
            First = f;
            Last = l;
        }
        
        public Name(string firstName, string mi, string lastName) : this(firstName, lastName)
        {
            var f = mi?.Trim();
            if (!IsValid(f))
            {
                throw new ArgumentException("Middle name is invalid");
            }
            MiddleInitial = f.Substring(0,1);
        }
        
        public Name FirstAndLastOnly() => new Name(this.First, this.Last);
        
        public static implicit operator string(Name n) => n.ToString();

        public override string ToString() => string.IsNullOrEmpty(MiddleInitial)
            ? $"{First} {Last}".CapitalizeFirstLetter()
            : $"{First} {MiddleInitial} {Last}".CapitalizeFirstLetter();

        private static bool IsValid(string s) => !string.IsNullOrEmpty(s);
    }
}