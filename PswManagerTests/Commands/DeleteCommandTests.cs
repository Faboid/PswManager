using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Extensions;
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

        readonly DeleteCommand delCommand = new DeleteCommand(TestsHelper.PswManager, TestsHelper.AutoInput);

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

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            string validName = TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name);

            yield return new object[] { ValidationCollection.ArgumentsNullMessage, null };

            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { null } };
            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { "" } };
            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { "     " } };

            yield return new object[] { new ValidationCollection(null).InexistentAccountMessage(), new string[] { "fakeAccountName" } };

            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, Array.Empty<string>() };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "eiwghrywhgi" } };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "tirhtewygh", "email@somewhere.com"} };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "tirhtewygh", "email@somewhere.com", "something"} };
        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
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
