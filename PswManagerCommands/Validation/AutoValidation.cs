using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation {
    public class AutoValidation<T> {

        readonly List<string> errors = new();
        readonly IReadOnlyList<PropertyInfo> properties;
        readonly IReadOnlyList<PropertyInfo> requiredProperties;
        readonly List<(ValidationLogic validator, List<PropertyInfo> props)> customValidators = new();

        public AutoValidation() {
            properties = typeof(T).GetProperties();
            requiredProperties = properties.Where(x => x.GetCustomAttribute<RequiredAttribute>() != null).ToList();
        }

        public void AddValidationAttribute(ValidationLogic validationLogic) {
            var props = properties.Where(x => x.GetCustomAttribute(validationLogic.GetAttributeType) != null).ToList();
            customValidators.Add((validationLogic, props));
        }

        public IReadOnlyList<string> GetErrors() {
            return errors;
        }

        public void Validate(T obj) {
            errors.Clear();
            RequiredPropertiesHaveValues(obj);

            //continue only if the required values are all filled
            if(errors.Any()) {
                return;
            }

            //todo - refactor this
            foreach(var (validator, props) in customValidators) {
                foreach(var prop in props) {
                    bool valid = validator.Validate(prop.GetCustomAttribute(validator.GetAttributeType), prop.GetValue(obj));
                    if(!valid) {
                        errors.Add("Temporary error message: value not valid");
                    }
                }
            }
        }

        private void RequiredPropertiesHaveValues(T obj) {

            //check they're not empty
            var emptyProps = requiredProperties.Where(x => string.IsNullOrEmpty((string)x.GetValue(obj)));

            //add error to list
            foreach(var s in emptyProps) {
                errors.Add($"You must provide a value for {s.Name}.");
            }
        }

    }
}
