using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.InputBuilder.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequestAttribute : Attribute {

        public string RequestMessage { get; private set; }
        public bool Optional { get; private set; }

        public RequestAttribute(string requestMessage) {
            RequestMessage = requestMessage;
            Optional = false;
        }

        public RequestAttribute(string requestMessage, bool optional) {
            RequestMessage = requestMessage;
            Optional = optional;
        }

    }
}
