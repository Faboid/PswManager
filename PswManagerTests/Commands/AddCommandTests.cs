using PswManagerCommands;
using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands;
using PswManagerTests.Commands.Helper;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Commands {

    [Collection("TestHelperCollection")]
    public class AddCommandTests {

        public AddCommandTests() {
            IDataFactory dataFactory = new DataFactory(TestsHelper.Paths);
            addCommand = new AddCommand(dataFactory.GetDataCreator(), TestsHelper.CryptoAccount);
            dataHelper = dataFactory.GetDataHelper();
            TestsHelper.SetUpDefault();
        }

        readonly IDataHelper dataHelper;
        readonly ICommand addCommand;

        [Theory]
        [InlineData("justSomeName#9839", "random@#[ssword", "random@email.it")]
        [InlineData("xmlnyyx", "ightueghtuy", "this@mail.com")]
        [InlineData("valueNamehere", "&&%£@#[][+*é", "valueNameHere@thisdomain.com")]
        public void AddSuccessfully(string name, string password, string email) {

            //arrange
            var obj = ClassBuilder.Build(addCommand, new List<string> { email, name, password });
            bool exists;
            CommandResult result;

            //act
            exists = dataHelper.AccountExist(name);
            result = addCommand.Run((ICommandInput)obj);

            //assert
            Assert.False(exists);
            Assert.True(result.Success);
            Assert.True(dataHelper.AccountExist(name));

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            static object[] NewObj(string errorMessage, string name, string password, string email) 
                => new object[] { 
                    errorMessage, 
                    ClassBuilder.Build(new AddCommand(null, null), new List<string> { email, name, password }) 
                };

            string existingName = TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name);
            //string validName = "someRandomNonexistentAccountName";

            //yield return new object[] { ValidationCollection.ArgumentsNullMessage, null };

            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { null } };
            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { validName, null, "email@this.it" } };
            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { validName, "fiehgywightuy", "      " } };
            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { validName, "", "email@this.com" } };

            yield return NewObj(AddCommand.AccountExistsErrorMessage, existingName, "somevalidPassword", "someValidEmail@email.com");

            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, Array.Empty<string>() };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName } };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "eiwghrywhgi" } };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "tirhtewygh", "email@somewhere.com", "oneTooMany" } };
        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, ICommandInput args) {

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
