using PswManager.Commands.Validation.Attributes;
using PswManager.Commands.Validation.Models;
using PswManagerLibrary.Commands.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.Validation.ValidationTypes {
    public class NoDuplicateValuesRule : ValidationRule {

        public NoDuplicateValuesRule() : base(typeof(NoDuplicateValuesAttribute), typeof(string)) { }

        protected override bool InnerLogic(RuleAttribute attribute, object value) {
            //as this check cares only that there's no duplicates, nulls are a-okay
            if(value == null) {
                return true;
            }

            var values = (value as string).Split(' ').Where(x => !string.IsNullOrWhiteSpace(x));
            return values.Distinct().Count() == values.Count();
        }
    }
}
