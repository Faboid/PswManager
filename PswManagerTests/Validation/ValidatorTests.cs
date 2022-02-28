﻿using PswManagerCommands.Validation;
using PswManagerCommands.Validation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Validation {
    public class ValidatorTests {

        public ValidatorTests() {
            autoValidator = new AutoValidationBuilder<TestObject>()
                .AddRule(new ValidateNotEmpty())
                .Build();

            ICondition<TestObject> ageCondition = new AgeCondition(2);
            minimumAgeMessage = ageCondition.GetErrorMessage();

            validator = new ValidatorBuilder<TestObject>()
                .AddAutoValidator(autoValidator)
                .AddCondition(new IndexHelper(0), (obj) => !string.IsNullOrWhiteSpace(obj.Name), missingNameMessage)
                .AddCondition(new IndexHelper(1, 0), (obj) => obj.Name.Length > 2, minimumNameLengthMessage)
                .AddCondition(ageCondition)
                .Build();
        }

        readonly string missingNameMessage = "Missing name.";
        readonly string minimumNameLengthMessage = "The name must be bigger than two characters.";
        readonly string minimumAgeMessage;

        readonly AutoValidation<TestObject> autoValidator;
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
            Assert.Contains("Temporary error message: value not valid", errors);

        }

    }

    public class TestObject {

        public TestObject(string id, string name, int age) {
            Id = id;
            Name = name;
            Age = age;
        }

        [NotEmpty]
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

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

        protected override bool InnerLogic(Attribute attribute, object value) {
            return !string.IsNullOrEmpty((string)value);
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotEmptyAttribute : Attribute { }

}
