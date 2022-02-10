using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation {

    public class AutoValidationBuilder<TObj> {

        private readonly IReadOnlyList<PropertyInfo> properties;
        readonly IReadOnlyList<PropertyInfo> requiredProperties;
        readonly List<(ValidationLogic validator, List<PropertyInfo> props)> customValidators = new();

        public AutoValidationBuilder() {
            properties = typeof(TObj).GetProperties().ToList();
            requiredProperties = properties.Where(x => x.GetCustomAttribute<RequiredAttribute>() != null).ToList();
        }

        public AutoValidationBuilder<TObj> AddLogic(ValidationLogic validationLogic) {
            var props = properties.Where(x => x.GetCustomAttribute(validationLogic.GetAttributeType) != null).ToList();
            customValidators.Add((validationLogic, props));
            return this;
        }

        public AutoValidation<TObj> Build() {
            return new AutoValidation<TObj>(requiredProperties, customValidators);
        }

    }
}
