using PswManager.Commands.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManager.Core.Commands.Validation.Attributes {
    public class ValidValuesAttribute : RuleAttribute {

        public string[] ValidValues { get; init; }

        public ValidValuesAttribute(string errorMessage, params string[] validValues) : base(errorMessage) {
            ValidValues = validValues;
        }

    }
}
