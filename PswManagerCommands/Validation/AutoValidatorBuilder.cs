using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation {

    public class AutoValidatorBuilder<TObj> {

        private readonly IReadOnlyList<PropertyInfo> properties;
        readonly IReadOnlyList<PropertyInfo> requiredProperties;
        readonly List<(ValidationRule validator, List<PropertyInfo> props)> customValidators = new();

        public AutoValidatorBuilder() {
            properties = typeof(TObj).GetProperties().ToList();
            requiredProperties = properties.Where(x => x.GetCustomAttribute<RequiredAttribute>() != null).ToList();
        }

        public AutoValidatorBuilder<TObj> AddRule(ValidationRule validationLogic) {
            var props = properties.Where(x => x.GetCustomAttribute(validationLogic.GetAttributeType) != null).ToList();
            customValidators.Add((validationLogic, props));
            return this;
        }

        public AutoValidator<TObj> Build() {
            return new AutoValidator<TObj>(requiredProperties, customValidators);
        }

    }
}
