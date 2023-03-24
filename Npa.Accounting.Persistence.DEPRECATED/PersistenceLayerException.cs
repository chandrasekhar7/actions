using System;
using System.Runtime.Serialization;

namespace Npa.Accounting.Persistence.DEPRECATED
{
    public class PersistenceLayerException : Exception
    {
        public PersistenceLayerException()
        {
        }

        public PersistenceLayerException(string? message) : base(message)
        {
        }

        public PersistenceLayerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PersistenceLayerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}