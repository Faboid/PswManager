using PswManagerCommands;
using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Builders;
using PswManagerCommands.Validation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Validation {
    public class AutoValidationBuilderTests {

        [Fact]
        public void ThrowWhenAddingAttributeOnInvalidDataType() {

            //arrange
            AutoValidatorBuilder<TestObj> autoValidatorBuilder = new();
            ValidationRule rule = new NotEmptyStringRule();

            //act

            //assert
            Assert.Throws<InvalidCastException>(() => autoValidatorBuilder.AddRule(rule));

        }


    }

    public class TestObj {

        [NotEmptyString("The given value is empty.")]
        public int Value { get; set; }

    }

    public class NotEmptyStringAttribute : RuleAttribute {
        public NotEmptyStringAttribute(string errorMessage) : base(errorMessage) {
        }
    }

    public class NotEmptyStringRule : ValidationRule {

        public NotEmptyStringRule() : base(typeof(NotEmptyStringAttribute), typeof(string)) {

        }

        protected override bool InnerLogic(RuleAttribute attribute, object value) {
            return !string.IsNullOrEmpty((string)value);
        }
    }

}
