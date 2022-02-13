using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.InputBuilder.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequestAttribute : Attribute {

        public string RequestMessage { get; set; }

        public RequestAttribute(string requestMessage) {
            RequestMessage = requestMessage;
        }

    }
}
