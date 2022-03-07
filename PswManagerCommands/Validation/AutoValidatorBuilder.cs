using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("PswManagerTests")]
namespace PswManagerCommands.Validation {

    public class AutoValidatorBuilder<TObj> {

        private readonly IReadOnlyList<PropertyInfo> properties;
        readonly IReadOnlyList<PropertyInfo> requiredProperties;
        readonly List<(ValidationRule validator, List<PropertyInfo> props)> customValidators = new();

        internal AutoValidatorBuilder() {
            properties = typeof(TObj).GetProperties().ToList();
            requiredProperties = properties.Where(x => x.GetCustomAttribute<RequiredAttribute>() != null).ToList();
        }

        public AutoValidatorBuilder<TObj> AddRule(ValidationRule validationLogic) {
            //todo - add a check to make sure ValidationRule.GetDataType's type is the same as the properties found below.
            var props = properties.Where(x => x.GetCustomAttribute(validationLogic.GetAttributeType) != null).ToList();
            customValidators.Add((validationLogic, props));
            return this;
        }

        public IAutoValidator<TObj> Build() {
            return new AutoValidator<TObj>(requiredProperties, customValidators);
        }

    }
}
