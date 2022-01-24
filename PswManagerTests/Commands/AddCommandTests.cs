using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Commands {

    [Collection("TestHelperCollection")]
    public class AddCommandTests {

        public AddCommandTests() {
            pswManager = TestsHelper.PswManager;
            IDataFactory dataFactory = new DataFactory(TestsHelper.Paths);
            addCommand = new AddCommand(dataFactory.GetDataCreator(), TestsHelper.CryptoAccount);
            TestsHelper.SetUpDefault();
        }

        readonly IPasswordManager pswManager;
        readonly ICommand addCommand;

        [Theory]
        [InlineData("justSomeName#9839", "random@#[ssword", "random@email.it")]
        [InlineData("xmlnyyx", "ightueghtuy", "this@mail.com")]
        [InlineData("valueNamehere", "&&%£@#[][+*é", "valueNameHere@thisdomain.com")]
        public void AddSuccessfully(string name, string password, string email) {

            //arrange
            var args = new string[] { name, password, email };
            bool exists;
            CommandResult result;

            //act
            exists = pswManager.AccountExist(name);
            result = addCommand.Run(args);

            //assert
            Assert.False(exists);
            Assert.True(result.Success);
            Assert.True(pswManager.AccountExist(name));

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            string validName = TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name);

            yield return new object[] { ValidationCollection.ArgumentsNullMessage, null };

            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { null } };
            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { validName, null, "email@this.it" } };
            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { validName, "fiehgywightuy", "      " } };
            yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { validName, "", "email@this.com" } };

            yield return new object[] { AddCommand.AccountExistsErrorMessage, new string[] { validName } };

            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, Array.Empty<string>() };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName } };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "eiwghrywhgi" } };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "tirhtewygh", "email@somewhere.com", "oneTooMany" } };
        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, params string[] args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = addCommand.Validate(args).success;
            result = addCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
