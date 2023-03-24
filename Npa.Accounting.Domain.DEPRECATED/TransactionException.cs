using System;
using System.Runtime.Serialization;

namespace Npa.Accounting.Domain.DEPRECATED
{
    public class TransactionException : DomainLayerException
    {
        public TransactionException()
        {
        }

        public TransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TransactionException(string message) : base(message)
        {
        }

        public TransactionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}