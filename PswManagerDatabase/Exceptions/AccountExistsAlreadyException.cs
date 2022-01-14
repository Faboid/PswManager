using System;
using System.Runtime.Serialization;

namespace PswManagerDatabase.Exceptions {
    public class AccountExistsAlreadyException : Exception {
        public AccountExistsAlreadyException() {
        }

        public AccountExistsAlreadyException(string message) : base(message) {
        }

        public AccountExistsAlreadyException(string message, Exception innerException) : base(message, innerException) {
        }

        protected AccountExistsAlreadyException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
