using PswManagerCommands.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation.Models {

    public abstract class ValidationRule {

        public Type GetAttributeType;
        public Type GetDataType;

        protected ValidationRule(Type attributeType, Type dataType) {
            if(!attributeType.IsSubclassOf(typeof(RuleAttribute))) {
                throw new InvalidCastException(nameof(attributeType));
            }

            GetAttributeType = attributeType;
            GetDataType = dataType;
        }

        public bool Validate(RuleAttribute attribute, object value) {
            if(GetAttributeType != attribute.GetType()) {
                throw new ArgumentException($"The given attribute is of the type {attribute.GetType()}, but this ValidationRule supports only {GetAttributeType}.", nameof(attribute));
            }
            if(GetDataType != value.GetType()) {
                throw new ArgumentException($"The given value is of the type {value.GetType()}, but this ValidationRule supports only {GetDataType}.", nameof(value));
            }

            return InnerLogic(attribute, value);
        }

        protected abstract bool InnerLogic(RuleAttribute attribute, object value);

    }

}
