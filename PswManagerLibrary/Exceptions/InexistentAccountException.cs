using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Exceptions {
    public class InexistentAccountException : InvalidCommandException {
        public InexistentAccountException(string message) : base(message) {
        }

        public InexistentAccountException(string command, string message) : base(command, message) {
        }

        public InexistentAccountException(string command, string message, Exception innerException) : base(command, message, innerException) {
        }
    }
}
