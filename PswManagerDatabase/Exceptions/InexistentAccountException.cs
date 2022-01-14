using System;
using System.Runtime.Serialization;

namespace PswManagerDatabase.Exceptions {
    public class InexistentAccountException : Exception {
        public InexistentAccountException() { }

        public InexistentAccountException(string message) : base(message) { }

        public InexistentAccountException(string message, Exception innerException) : base(message, innerException) { }

        protected InexistentAccountException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
