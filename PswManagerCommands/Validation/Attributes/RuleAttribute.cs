using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManager.Commands.Validation.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class RuleAttribute : Attribute {

        public RuleAttribute(string errorMessage) {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
    }
}
