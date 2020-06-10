using System;
using System.Runtime.Serialization;

namespace Retries
{
    [Serializable]
    public class ExternalServiceUnavailableException : Exception
    {
        public ExternalServiceUnavailableException()
        {
        }

        public ExternalServiceUnavailableException(string message) : base(message)
        {
        }

        public ExternalServiceUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExternalServiceUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}