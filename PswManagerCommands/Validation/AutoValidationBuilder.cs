using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation {

    public class AutoValidationBuilder<TObjType> {

        private IReadOnlyList<PropertyInfo> properties = typeof(TObjType).GetProperties().ToList();
        readonly IReadOnlyList<PropertyInfo> requiredProperties;
        readonly List<(ValidationLogic validator, List<PropertyInfo> props)> customValidators = new();

        public AutoValidationBuilder() {
            requiredProperties = properties.Where(x => x.GetCustomAttribute<RequiredAttribute>() != null).ToList();
        }

        public AutoValidationBuilder<TObjType> AddLogic(ValidationLogic validationLogic) {
            var props = properties.Where(x => x.GetCustomAttribute(validationLogic.GetAttributeType) != null).ToList();
            customValidators.Add((validationLogic, props));
            return this;
        }

        public AutoValidation<TObjType> Build() {
            return new AutoValidation<TObjType>(requiredProperties, customValidators);
        }

    }
}
