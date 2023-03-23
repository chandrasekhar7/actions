﻿using System;
using System.Runtime.Serialization;

namespace Npa.Accounting.Common.ErrorHandling
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException()
        {
        }

        public AccessDeniedException(string? message) : base(message)
        {
        }

        public AccessDeniedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}