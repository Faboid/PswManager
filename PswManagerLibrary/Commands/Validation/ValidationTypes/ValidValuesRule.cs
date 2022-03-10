using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using PswManagerLibrary.Commands.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.Validation.ValidationTypes {
    internal class ValidValuesRule : ValidationRule {

        public ValidValuesRule() : base(typeof(ValidValuesAttribute), typeof(string)) {

        }

        protected override bool InnerLogic(RuleAttribute attribute, object value) {
            var validKeys = (attribute as ValidValuesAttribute).ValidValues;

            return (value as string)
                .Split(' ')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .All(x => validKeys.Contains(x));
        }
    }
}
