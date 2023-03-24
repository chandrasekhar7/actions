using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Domain.DEPRECATED.Transactions
{
    public record CardValidationResult(CardReturnStatus ReturnStatus,bool CanDisburse,string? ReturnMessage, bool ShouldDelete)
    {
        
    }
}