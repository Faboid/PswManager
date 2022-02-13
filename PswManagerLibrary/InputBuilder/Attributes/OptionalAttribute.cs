using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.InputBuilder.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OptionalAttribute : Attribute {

        public string RequestMessage { get; set; }

        public OptionalAttribute(string requestMessage) {
            RequestMessage = $"(Optional){requestMessage}";
        }

    }
}
