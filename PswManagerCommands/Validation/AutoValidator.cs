using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PswManagerCommands.Validation {

    internal class AutoValidator<T> : IAutoValidator<T> {

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
                    RuleAttribute attribute = (RuleAttribute)prop.GetCustomAttribute(validator.GetAttributeType);
                    bool valid = validator.Validate(attribute, prop.GetValue(obj));
                    if(!valid) {
                        yield return attribute.ErrorMessage;
                    }
                }
            }
        }

        private IEnumerable<string> RequiredPropertiesHaveValues(T obj) {

            //check they're not empty
            var emptyProps = requiredProperties.Where(x => string.IsNullOrEmpty((string)x.GetValue(obj)));

            //add error to list
            foreach(var prop in emptyProps) {
                yield return prop.GetCustomAttribute<RequiredAttribute>().GetErrorMessage(prop);
            }
        }

    }
}
