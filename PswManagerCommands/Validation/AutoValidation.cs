using PswManagerCommands.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation {
    public class AutoValidation<T> {

        readonly List<string> errors = new();
        readonly IReadOnlyList<PropertyInfo> requiredProperties;

        public AutoValidation() {
            requiredProperties = typeof(T).GetProperties().Where(x => x.GetCustomAttribute<RequiredAttribute>() != null).ToList();
        }

        public IReadOnlyList<string> GetErrors() {
            return errors;
        }

        public void Validate(T obj) {
            RequiredPropertiesHaveValues(obj);
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
