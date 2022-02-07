using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.Validation.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class VerifyAccountExistenceAttribute : Attribute {

        public bool ShouldExist;

        public VerifyAccountExistenceAttribute(bool shouldExist) { 
            ShouldExist = shouldExist;
        }

    }
}
