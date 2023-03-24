using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[assembly:InternalsVisibleTo("Npa.Accounting.Tests")]
namespace Npa.Accounting.Domain.DEPRECATED
{
    public class DomainLayerException : Exception
    {
        public DomainLayerException()
        {
        }

        public DomainLayerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DomainLayerException(string message) : base(message)
        {
        }

        public DomainLayerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}