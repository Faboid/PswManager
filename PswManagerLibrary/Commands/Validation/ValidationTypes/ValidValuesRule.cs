using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using PswManagerLibrary.Commands.Validation.Attributes;
using System;
using System.Linq;

namespace PswManagerLibrary.Commands.Validation.ValidationTypes {
    internal class ValidValuesRule : ValidationRule {

        public ValidValuesRule() : base(typeof(ValidValuesAttribute), typeof(string)) {

        }

        protected override bool InnerLogic(RuleAttribute attribute, object value) {

            //as this only cares on whether the values within are valid,
            //null is a-okay, as it expresses the "lack" of values
            if(value == null) {
                return true;
            }

            var validKeys = (attribute as ValidValuesAttribute).ValidValues;

            return (value as string)
                .Split(' ')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .All(x => validKeys.Contains(x));
        }
    }
}
