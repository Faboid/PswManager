using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.InputBuilder.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequestAttribute : Attribute {

        public string DisplayName { get; private set; }
        public string RequestMessage { get; private set; }
        public bool Optional { get; private set; }

        public RequestAttribute(string displayName, string requestMessage) {
            DisplayName = displayName;
            RequestMessage = requestMessage;
            Optional = false;
        }

        public RequestAttribute(string displayName, string requestMessage, bool optional) {
            DisplayName = displayName;
            RequestMessage = requestMessage;
            Optional = optional;
        }

    }
}
