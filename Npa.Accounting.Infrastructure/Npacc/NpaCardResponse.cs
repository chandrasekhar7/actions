namespace Npa.Accounting.Infrastructure.Npacc
{
    public class NpaCardResponse
    {
        public int TokenId { get; init; }
        public string ReturnCode { get; init; }
        public string? ReturnMessage { get; init; }
        public string? RefNum { get; init; }
    }
}