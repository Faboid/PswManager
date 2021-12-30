using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Extensions;
using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace PswManagerTests.Commands {
    [Collection("TestHelperCollection")]
    public class EditCommandTests {

        readonly EditCommand editCommand = new EditCommand(TestsHelper.PswManager);

        public static IEnumerable<object[]> EditSuccessfullyData() {
            var def = TestsHelper.DefaultValues;

            //[0]args
            //[1]newName
            //[2]expected return string
            yield return new object[] {
                new[] { def.GetValue(1, DefaultValues.TypeValue.Name), "password:newPassword1", "email:newEmail1" },
                def.GetValue(1, DefaultValues.TypeValue.Name),
                $"{def.GetValue(1, DefaultValues.TypeValue.Name)} newPassword1 newEmail1"
            };
            yield return new object[] {
                new[] { def.GetValue(2, DefaultValues.TypeValue.Name), "name:newName2", "email:newEmail2" },
                "newName2",
                $"newName2 {def.GetValue(2, DefaultValues.TypeValue.Password)} newEmail2"
            };
            yield return new object[] {
                new[] { def.GetValue(3, DefaultValues.TypeValue.Name), "name:fixedName", "password:passfix", "email:fixed@email.com" },
                "fixedName",
                "fixedName passfix fixed@email.com"
            };
        }

        [Theory]
        [MemberData(nameof(EditSuccessfullyData))]
        public void EditSuccessfully(string[] args, string newName, string expected) {

            //arrange
            TestsHelper.SetUpDefault();
            string previous;
            string actual;

            //act
            previous = TestsHelper.PswManager.GetPassword(args[0]);
            editCommand.Run(args);
            actual = TestsHelper.PswManager.GetPassword(newName);

            //assert
            Assert.NotEqual(previous, actual);
            Assert.Equal(expected, actual);

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            string validName = TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name);

            yield return new object[] { ValidationCollection.ArgumentsNullMessage, null };

            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { null } };
            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { "", "name:newvalue" } };
            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { "     ", "password:tiehwgfuh" } };
            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { validName, "password:tiehwgfuh", null } };

            yield return new object[] { new ValidationCollection(null).InexistentAccountMessage(), new string[] { "fakeAccountName" } };

            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, Array.Empty<string>() };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName } };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "password:newPassword1", "email:email@somewhere.com", "name:newName", "password:newvalidPassword" } };

            //todo - insert syntax check
        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, params string[] args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = editCommand.Validate(args).success;
            result = editCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
