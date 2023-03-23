using Npa.Accounting.Tests.Persistence.Fixtures;
using Xunit;

namespace Npa.Accounting.Tests.Persistence.DbContexts
{
    [CollectionDefinition("PaymentDbContextTests")]
    public class PaymentDbContextTestsCollection : ICollectionFixture<TransactionDbFixture>
    {
        
    }
}