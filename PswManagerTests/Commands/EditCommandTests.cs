using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Extensions;
using PswManagerLibrary.Storage;
using PswManagerTests.Commands.Helper;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace PswManagerTests.Commands {
    [Collection("TestHelperCollection")]
    public class EditCommandTests {

        public EditCommandTests() {
            IDataFactory dataFactory = new DataFactory(TestsHelper.Paths);
            editCommand = new EditCommand(dataFactory.GetDataEditor(), TestsHelper.CryptoAccount);
            getCommand = new GetCommand(dataFactory.GetDataReader(), TestsHelper.CryptoAccount);
        }

        readonly GetCommand getCommand;
        readonly EditCommand editCommand;

        public static IEnumerable<object[]> EditSuccessfullyData() {
            static object[] NewObj(string name, string newName, string newPassword, string newEmail, string expected)
                => new object[] {
                    name,
                    newName ?? name,
                    ClassBuilder.Build(new EditCommand(null, null), new List<string> { newPassword, newName, newEmail, name }),
                    expected
                };

            var def = TestsHelper.DefaultValues;

            yield return NewObj(def.GetValue(1, DefaultValues.TypeValue.Name), 
                null, "newPassword1", "newEmail1", 
                $"{def.GetValue(1, DefaultValues.TypeValue.Name)} newPassword1 newEmail1");

            yield return NewObj(def.GetValue(2, DefaultValues.TypeValue.Name),
                "new:Name2", null, "newEmail2",
                $"new:Name2 {def.GetValue(2, DefaultValues.TypeValue.Password)} newEmail2"
                );

            yield return NewObj(def.GetValue(3, DefaultValues.TypeValue.Name),
                "fixedName", "passfix", "fixed@email.com",
                "fixedName passfix fixed@email.com");

        }

        [Theory]
        [MemberData(nameof(EditSuccessfullyData))]
        public void EditSuccessfully(string name, string newName, ICommandInput args, string expected) {

            //arrange
            TestsHelper.SetUpDefault();

            //act
            var previous = getCommand.Run(ClassBuilder.Build(getCommand, new List<string>() { name }));
            editCommand.Run(args);
            var actual = getCommand.Run(ClassBuilder.Build(getCommand, new List<string>() { newName ?? name }));

            //assert
            Assert.NotEqual(previous, actual);
            Assert.Equal(expected, actual.QueryReturnValue);

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            static object[] NewObj(string errorMessage, string name, string newName, string newPassword, string newEmail)
                => new object[] {
                    errorMessage,
                    ClassBuilder.Build(new EditCommand(null, null), new List<string> { newPassword, newName, newEmail, name })
                };
            //string validName = TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name);

            //yield return new object[] { ValidationCollection.ArgumentsNullMessage, null };

            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { null } };
            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { "", "name:newvalue" } };
            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { "     ", "password:tiehwgfuh" } };
            //yield return new object[] { ValidationCollection.ArgumentsNullOrEmptyMessage, new string[] { validName, "password:tiehwgfuh", null } };

            yield return NewObj(new ValidationCollection<object>(null).InexistentAccountMessage(), "fakeAccountName", null, "newPasshere", null);

            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, Array.Empty<string>() };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName } };
            //yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "password:newPassword1", "email:email@somewhere.com", "name:newName", "password:newvalidPassword" } };

            //yield return new object[] { EditCommand.InvalidSyntaxMessage, new string[] { validName, "newPassword" } };
            //yield return new object[] { EditCommand.InvalidSyntaxMessage, new string[] { validName, "pass@eqwwr" } };

            //yield return new object[] { EditCommand.InvalidKeyFound, new string[] { validName, "password:qrqrewqe", "nam:newname" } };
            //yield return new object[] { EditCommand.InvalidKeyFound, new string[] { validName, "name:newname", "password:qweqwed" , "ema:email@thisone.com"} };

            //yield return new object[] { EditCommand.DuplicateKeyFound, new string[] { validName, "email:email@somewhere.com", "email:someEma@here.com" } };
            //yield return new object[] { EditCommand.DuplicateKeyFound, new string[] { validName, "password:newPassword1", "email:email@somewhere.com", "password:newvalidPassword" } };
        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, ICommandInput args) {

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
