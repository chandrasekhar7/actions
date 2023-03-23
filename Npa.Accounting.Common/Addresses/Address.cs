


namespace Npa.Accounting.Common.Addresses
{
    public record Address(string Street1, string City, State State, ZipCode ZipCode, string? Street2 = default)
    {
        public void Deconstruct(out string street, out string city, out string state, out string zip, out string? street2)
        {
            street = Street1;
            city = City;
            state = State.ToString();
            zip = ZipCode.ToString();
            street2 = Street2;
        }
        public override string ToString()
        {
            var apt = !string.IsNullOrWhiteSpace(Street2) ? " " + Street2 : "";
            return $"{Street1}{apt}, {City}, {State} {ZipCode}";
        }
    }
}