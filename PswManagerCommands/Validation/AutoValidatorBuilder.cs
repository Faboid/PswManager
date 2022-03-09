using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using System;
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
            var props = properties.Where(x => x.GetCustomAttribute(validationLogic.GetAttributeType) != null).ToList();
            props.ForEach(x => ValidateProperty(x, validationLogic));
            customValidators.Add((validationLogic, props));
            return this;
        }

        public AutoValidatorBuilder<TObj> AddRule<TValidationRule>() where TValidationRule : ValidationRule, new() {
            var validationLogic = new TValidationRule();
            return AddRule(validationLogic);
        }

        public IAutoValidator<TObj> Build() {
            return new AutoValidator<TObj>(requiredProperties, customValidators);
        }

        /// <summary>
        /// This method makes sure the attributes are placed in the correct data types and will throw if they're not.
        /// </summary>
        private void ValidateProperty(PropertyInfo property, ValidationRule validationLogic) {

            if(property.PropertyType == validationLogic.GetDataType) {
                return;
            }

            throw new InvalidCastException(
                $"The object {nameof(TObj)} uses the attribute {validationLogic.GetAttributeType} for property {property.Name} of type {property.PropertyType}, " +
                $"but the ValidationRule given only supports the type {validationLogic.GetDataType}."
                );
        }

    }
}
