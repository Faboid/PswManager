using PswManager.Commands.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.Validation.Attributes {
    public class NoDuplicateValuesAttribute : RuleAttribute {

        public NoDuplicateValuesAttribute(string errorMessage) : base(errorMessage) {

        }

    }
}
