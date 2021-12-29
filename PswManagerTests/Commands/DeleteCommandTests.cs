using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerLibrary.Commands;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Commands {

    [Collection("TestHelperCollection")]
    public class DeleteCommandTests {

        DeleteCommand delCommand = new DeleteCommand(TestsHelper.PswManager, TestsHelper.AutoInput);

        [Fact]
        public void DeleteSuccessfully() {

            //arrange
            TestsHelper.SetUpDefault();
            string name = TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name);

            //act
            bool exist = TestsHelper.PswManager.AccountExist(name);
            delCommand.Run(new string[] { name });

            //assert
            Assert.True(exist);
            Assert.False(TestsHelper.PswManager.AccountExist(name));

        }

        [Theory]
        [InlineData(ValidationCollection.ArgumentsNullMessage, null)]
        [InlineData(ValidationCollection.ArgumentsNullOrEmptyMessage, null)]
        [InlineData(ValidationCollection.WrongArgumentsNumberMessage)]
        [InlineData(ValidationCollection.WrongArgumentsNumberMessage, "expectedName", "oneTooMany")]
        [InlineData(ValidationCollection.WrongArgumentsNumberMessage, "expectedName", "oneTooMany", "twoTooMany")]
        [InlineData(ValidationCollection.WrongArgumentsNumberMessage, "expectedName", "oneTooMany", "twoTooMany", "threeTooMany")]
        [InlineData("The given account doesn't exist.", "fakeNonExistentAccount")]
        public void ExpectedValidationFailures(string expectedErrorMessage, params string[] args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = delCommand.Validate(args).success;
            result = delCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
