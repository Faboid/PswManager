using PswManager.Commands.Validation.Attributes;
using PswManager.Commands.Validation.Builders;
using PswManager.Commands.Validation.Models;
using PswManager.Commands.Validation.Validators;
using System;
using Xunit;

namespace PswManager.Tests.Validation {

    public class AutoValidationTests {

        public AutoValidationTests() {
            autoVal = new AutoValidatorBuilder<ValidObject>()
                .AddRule(new VerifyRangeLogic())
                .Build();
        }

        readonly IAutoValidator<ValidObject> autoVal;

        [Fact]
        public void ValidationSuccess() {

            //arrange
            ValidObject obj = new();
            obj.Name = "Hello!";
            obj.Password = "igghrtuh";
            obj.Email = "yoyo@email.com";
            obj.Number = 5;

            //act
            var result = autoVal.Validate(obj);

            //assert
            Assert.Empty(result);

        }

        [Fact]
        public void ValidationFailure_MissingRequired() {

            //arrange
            ValidObject obj = new();
            obj.Name = "Hello!";

            //act
            var result = autoVal.Validate(obj);

            //assert
            Assert.NotEmpty(result);

        }

        [Fact]
        public void ValidationFailure_MissingCustom() {

            //arrange
            ValidObject obj = new();
            obj.Name = "Hello!";
            obj.Password = "igghrtuh";
            obj.Email = "yoyo@email.com";
            obj.Number = -15;

            //act
            var result = autoVal.Validate(obj);

            //assert
            Assert.NotEmpty(result);

        }

    }

    internal class ValidObject {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(0, 50, "Number should be between 0 and 50.")]
        public int Number { get; set; }

    }

    internal class VerifyRangeLogic : ValidationRule {
        public VerifyRangeLogic() : base(typeof(RangeAttribute), typeof(int)) { }

        protected override bool InnerLogic(RuleAttribute att, object value) {
            var attribute = (RangeAttribute)att;
            var val = (int)value;
            return val >= attribute.Min && val <= attribute.Max;
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class RangeAttribute : RuleAttribute {

        public int Min;
        public int Max;

        public RangeAttribute(int min, int max, string errorMessage) : base(errorMessage) {
            Max = max;
            Min = min;
        }

    }

}
