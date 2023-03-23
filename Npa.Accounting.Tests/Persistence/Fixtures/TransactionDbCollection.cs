using Xunit;

namespace Npa.Accounting.Tests.Persistence.Fixtures
{
    [CollectionDefinition("TransactionDb")]
    public class TransactionDbCollection : ICollectionFixture<TransactionDbFixture>
    {
        
    }
}