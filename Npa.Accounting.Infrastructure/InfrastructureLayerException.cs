using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[assembly: InternalsVisibleTo("Npa.Accounting.Tests")]
namespace Npa.Accounting.Infrastructure
{
    public class InfrastructureLayerException : Exception
    {
        public InfrastructureLayerException()
        {
        }

        public InfrastructureLayerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InfrastructureLayerException(string message) : base(message)
        {
        }

        public InfrastructureLayerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}