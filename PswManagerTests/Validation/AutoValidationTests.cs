using PswManagerCommands.Validation;
using PswManagerCommands.Validation.Attributes;
using PswManagerCommands.Validation.Models;
using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands.Validation.Attributes;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Validation {

    public class AutoValidationTests {

        [Fact]
        public void ValidationSuccess() {

            //arrange
            AutoValidation<ValidObject> autoVal = new();
            autoVal.AddValidationAttribute(new VerifyRangeLogic());
            ValidObject obj = new();
            obj.Name = "Hello!";
            obj.Password = "igghrtuh";
            obj.Email = "yoyo@email.com";
            obj.Number = 5;

            //act
            autoVal.Validate(obj);
            var result = autoVal.GetErrors();

            //assert
            Assert.Empty(result);

        }

        [Fact]
        public void ValidationFailure_MissingRequired() {

            //arrange
            AutoValidation<ValidObject> autoVal = new();
            autoVal.AddValidationAttribute(new VerifyRangeLogic());
            ValidObject obj = new();
            obj.Name = "Hello!";

            //act
            autoVal.Validate(obj);
            var result = autoVal.GetErrors();

            //assert
            Assert.NotEmpty(result);

        }

        [Fact]
        public void ValidationFailure_MissingCustom() {

            //arrange
            AutoValidation<ValidObject> autoVal = new();
            autoVal.AddValidationAttribute(new VerifyRangeLogic());
            ValidObject obj = new ValidObject();
            obj.Name = "Hello!";
            obj.Password = "igghrtuh";
            obj.Email = "yoyo@email.com";
            obj.Number = -15;

            //act
            autoVal.Validate(obj);
            var result = autoVal.GetErrors();

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

        [Range(0, 50)]
        public int Number { get; set; }

    }

    internal class VerifyRangeLogic : ValidationLogic {
        public VerifyRangeLogic() : base(typeof(RangeAttribute), typeof(int)) { }

        protected override bool InnerLogic(Attribute att, object value) {
            var attribute = (RangeAttribute)att;
            var val = (int)value;
            return val >= attribute.Min && val <= attribute.Max;
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class RangeAttribute : Attribute {

        public int Min;
        public int Max;

        public RangeAttribute(int min, int max) {
            Max = max;
            Min = min;
        }

    }

}
