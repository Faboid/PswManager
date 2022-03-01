using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation {
    public class AutoValidator<T> {

        readonly IReadOnlyList<PropertyInfo> requiredProperties;
        readonly List<(ValidationRule validator, List<PropertyInfo> props)> customRules = new();

        internal AutoValidator(IReadOnlyList<PropertyInfo> required, List<(ValidationRule validator, List<PropertyInfo> props)> custom) {
            requiredProperties = required;
            customRules = custom;
        }

        public IEnumerable<string> Validate(T obj) {
            var errors = RequiredPropertiesHaveValues(obj);
            foreach(var e in errors) {
                yield return e;
            }

            //if any required property is missing, return early.
            //This is to avoid redundant error messages
            if(errors.Any()) {
                yield break;
            }

            //todo - refactor this
            foreach(var (validator, props) in customRules) {
                foreach(var prop in props) {
                    bool valid = validator.Validate(prop.GetCustomAttribute(validator.GetAttributeType), prop.GetValue(obj));
                    if(!valid) {
                        yield return "Temporary error message: value not valid";
                    }
                }
            }
        }

        private IEnumerable<string> RequiredPropertiesHaveValues(T obj) {

            //check they're not empty
            var emptyProps = requiredProperties.Where(x => string.IsNullOrEmpty((string)x.GetValue(obj)));

            //add error to list
            foreach(var s in emptyProps) {
                yield return $"You must provide a value for {s.Name}.";
            }
        }

    }
}
