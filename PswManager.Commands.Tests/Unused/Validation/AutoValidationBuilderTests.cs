using PswManager.Commands.Validation.Attributes;
using PswManager.Commands.Validation.Builders;
using PswManager.Commands.Validation.Models;
using Xunit;

namespace PswManager.Commands.Tests.Unused.Validation; 
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

