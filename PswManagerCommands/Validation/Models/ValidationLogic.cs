using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation.Models {
    public abstract class ValidationLogic<TAttribute, TDataType> where TAttribute: Attribute {

        public Type GetAttributeType => typeof(TAttribute);
        public Type GetDataType => typeof(TDataType);

        public abstract bool Validate(TAttribute attribute, TDataType value, out string errorMessage);

    }
}
