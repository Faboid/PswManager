using PswManagerCommands.Validation;
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

            ValidatorBuilder<TestObject> builder = new();
            Validator<TestObject> validator = builder
                .AddCondition(new IndexHelper(0), (obj) => !string.IsNullOrWhiteSpace(obj.Name), missingNameMessage)
                .AddCondition(new IndexHelper(1, 0), (obj) => obj.Name.Length > 2, minimumNameLengthMessage)
                .AddCondition(ageCondition)
                .Build();

            TestObject obj1 = new("rightuy", "Name here", 15);
            TestObject obj2 = new("rigrrree", "y", 3);

            //act
            var err_1 = validator.Validate(obj1);
            var err_2 = validator.Validate(obj2);

            //assert
            Assert.Empty(err_1);
            Assert.NotEmpty(err_2);
            Assert.Contains(minimumNameLengthMessage, err_2);
            Assert.Contains(minimumAgeMessage, err_2);

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

        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

    }
}
