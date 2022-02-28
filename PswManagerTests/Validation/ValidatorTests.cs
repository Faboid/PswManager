using PswManagerCommands.Validation;
using PswManagerCommands.Validation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Validation {
    public class ValidatorTests {

        [Fact]
        public void ValidateSuccessfully() {

            //arrange
            string missingNameMessage = "Missing name.";
            string minimumNameLengthMessage = "The name must be bigger than two characters.";
            string minimumAgeMessage = "The minimum required age is 13.";
            Condition<TestObject> ageCondition = new(new IndexHelper(2), (obj) => obj.Age > 13, minimumAgeMessage);


            AutoValidationBuilder<TestObject> autoValBuilder = new();
            AutoValidation<TestObject> autoValidator = autoValBuilder
                .AddRule(new ValidateNotEmpty())
                .Build();

            ValidatorBuilder<TestObject> builder = new();
            Validator<TestObject> validator = builder
                .AddAutoValidator(autoValidator)
                .AddCondition(new IndexHelper(0), (obj) => !string.IsNullOrWhiteSpace(obj.Name), missingNameMessage)
                .AddCondition(new IndexHelper(1, 0), (obj) => obj.Name.Length > 2, minimumNameLengthMessage)
                .AddCondition(ageCondition)
                .Build();

            TestObject obj1 = new("rightuy", "Name here", 15);
            TestObject obj2 = new("rigrrree", "y", 3);
            TestObject obj3 = new("", "validName", 77);

            //act
            var err_1 = validator.Validate(obj1);
            var err_2 = validator.Validate(obj2);
            var err_3 = validator.Validate(obj3);

            //assert
            Assert.Empty(err_1);
            Assert.NotEmpty(err_2);
            Assert.Contains(minimumNameLengthMessage, err_2);
            Assert.Contains(minimumAgeMessage, err_2);
            Assert.NotEmpty(err_3);
            Assert.Contains("Temporary error message: value not valid", err_3);

        }


    }

    public class TestObject {

        public TestObject() {
             
        }

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

    public class ValidateNotEmpty : ValidationRule {
        public ValidateNotEmpty() : base(typeof(NotEmptyAttribute), typeof(string)) { }

        protected override bool InnerLogic(Attribute attribute, object value) {
            return !string.IsNullOrEmpty((string)value);
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotEmptyAttribute : Attribute { }

}
