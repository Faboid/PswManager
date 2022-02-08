using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation.Models {

    public abstract class ValidationLogic {

        public Type GetAttributeType;
        public Type GetDataType;

        protected ValidationLogic(Type attributeType, Type dataType) {
            GetAttributeType = attributeType;
            GetDataType = dataType;
        }

        public bool Validate(Attribute attribute, object value) {
            if(GetAttributeType != attribute.GetType()) {
                throw new ArgumentException("The given attribute is not the same as the one given to the constructor.", nameof(attribute));
            }
            if(GetDataType != value.GetType()) {
                throw new ArgumentException("The given value is not the same as the one given to the constructor.", nameof(value));
            }

            return InnerLogic(attribute, value);
        }

        protected abstract bool InnerLogic(Attribute attribute, object value);

    }

}
