using PswManager.Commands.Validation.Attributes;
using PswManager.Commands.Validation.Builders;
using PswManager.Commands.Validation.Models;
using PswManager.Commands.Validation.Validators;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManager.Tests.Validation {
    public class ValidatorTests {

        public ValidatorTests() {
            autoValidatorNotEmpty = new AutoValidatorBuilder<TestObject>()
                .AddRule(new ValidateNotEmpty())
                .Build();

            autoValidatorLessThanOneHundred = new AutoValidatorBuilder<TestObject>()
                .AddRule(new ValidateLessThanOneHundred())
                .Build();

            ICondition<TestObject> ageCondition = new AgeCondition(2);
            minimumAgeMessage = ageCondition.GetErrorMessage();

            validator = new ValidatorBuilder<TestObject>()
                .AddAutoValidator(autoValidatorNotEmpty)
                .AddAutoValidator(autoValidatorLessThanOneHundred)
                .AddCondition(0, (obj) => !string.IsNullOrWhiteSpace(obj.Name), missingNameMessage)
                .AddCondition(new IndexHelper(1, 0), (obj) => obj.Name.Length > 2, minimumNameLengthMessage)
                .AddCondition(ageCondition)
                .Build();
        }

        readonly string missingNameMessage = "Missing name.";
        readonly string minimumNameLengthMessage = "The name must be bigger than two characters.";
        readonly string minimumAgeMessage;

        readonly IAutoValidator<TestObject> autoValidatorNotEmpty;
        readonly IAutoValidator<TestObject> autoValidatorLessThanOneHundred;
        readonly IValidator<TestObject> validator;

        [Fact]
        public void Success() {

            //arrange
            TestObject obj = new("rightuy", "Name here", 15);

            //act
            var errors = validator.Validate(obj);

            //assert
            Assert.Empty(errors);
        }

        [Fact]
        public void Failure_From_CustomConditions() {

            //arrange
            TestObject obj = new("rigrrree", "y", 3);

            //act
            var errors = validator.Validate(obj);

            //assert
            Assert.NotEmpty(errors);
            Assert.Contains(minimumNameLengthMessage, errors);
            Assert.Contains(minimumAgeMessage, errors);

        }

        [Fact]
        public void Failure_From_AutoValidation() {

            //arrange
            TestObject obj = new("", "validName", 77);

            //act
            var errors = validator.Validate(obj);

            //assert
            Assert.NotEmpty(errors);
            Assert.Contains("Id cannot be empty.", errors);

        }

        [Fact]
        public void Failure_From_SecondAutoValidation() {

            //arrange
            TestObject obj = new("righteight", "Yereth", 177);

            //act
            var errors = validator.Validate(obj);

            //assert
            Assert.NotEmpty(errors);
            Assert.Contains("Age cannot be less than one hundred.", errors);

        }

    }

    public class TestObject {

        public TestObject(string id, string name, int age) {
            Id = id;
            Name = name;
            Age = age;
        }

        [NotEmpty("Id")] public string Id { get; set; }
        public string Name { get; set; }
        [LessThanOneHundred("Age")] public int Age { get; set; }

    }

    public class AgeCondition : ICondition<TestObject> {

        public AgeCondition(int index) {
            this.index = index;
        }

        private readonly int index;

        public string GetErrorMessage() => "The minimum required age is 13.";

        public bool IsValid(TestObject obj) => IsValid(obj, new List<int>());

        public bool IsValid(TestObject obj, IList<int> failedConditions) {

            try {

                //if the validation passes, return true
                if(obj.Age > 13) {
                    return true;
                }
            }
            catch {

                return false;
            }

            failedConditions.Add(index);
            return false;
        }
    }

    public class ValidateNotEmpty : ValidationRule {
        public ValidateNotEmpty() : base(typeof(NotEmptyAttribute), typeof(string)) { }

        protected override bool InnerLogic(RuleAttribute attribute, object value) {
            return !string.IsNullOrEmpty((string)value);
        }
    }

    public class ValidateLessThanOneHundred : ValidationRule {

        public ValidateLessThanOneHundred() : base(typeof(LessThanOneHundredAttribute), typeof(int)) { }

        protected override bool InnerLogic(RuleAttribute attribute, object value) {
            return (int)value < 100;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotEmptyAttribute : RuleAttribute {
        public NotEmptyAttribute(string name) : base($"{name} cannot be empty.") {

        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class LessThanOneHundredAttribute : RuleAttribute {
        public LessThanOneHundredAttribute(string name) : base($"{name} cannot be less than one hundred.") {

        }
    }

}
