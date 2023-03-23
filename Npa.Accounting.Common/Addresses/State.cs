

namespace Npa.Accounting.Common.Addresses
{
    public record State(string Abbreviation, string? Name = default)
    {
        public static explicit operator State(string value) => new State(value);

        public static implicit operator string(State state) => state.Abbreviation;

        public override string ToString() => Abbreviation;
    }
}