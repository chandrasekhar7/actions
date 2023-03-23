using System;
using System.Runtime.Serialization;

namespace Npa.Accounting.Common.ErrorHandling;

public class CriticalErrorException : Exception
{
    public CriticalErrorException()
    {
    }

    public CriticalErrorException(string? message) : base(message)
    {
    }

    public CriticalErrorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected CriticalErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}