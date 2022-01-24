using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
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

        public EditCommandTests() {
            IDataFactory dataFactory = new DataFactory(TestsHelper.Paths);
            editCommand = new EditCommand(dataFactory.GetDataEditor(), TestsHelper.CryptoAccount);
            getCommand = new GetCommand(dataFactory.GetDataReader(), TestsHelper.CryptoAccount);
        }

        readonly GetCommand getCommand;
        readonly EditCommand editCommand;

        public static IEnumerable<object[]> EditSuccessfullyData() {
            var def = TestsHelper.DefaultValues;

            //[0]args
            //[1]newName
            //[2]expected return string
            yield return new object[] {
                new[] { def.GetValue(1, DefaultValues.TypeValue.Name), "email:newEmail1", "password:newPassword1" },
                def.GetValue(1, DefaultValues.TypeValue.Name),
                $"{def.GetValue(1, DefaultValues.TypeValue.Name)} newPassword1 newEmail1"
            };
            yield return new object[] {
                new[] { def.GetValue(2, DefaultValues.TypeValue.Name), "name:new:Name2", "email:newEmail2" },
                "new:Name2",
                $"new:Name2 {def.GetValue(2, DefaultValues.TypeValue.Password)} newEmail2"
            };
            yield return new object[] {
                new[] { def.GetValue(3, DefaultValues.TypeValue.Name), "password:passfix", "name:fixedName", "email:fixed@email.com" },
                "fixedName",
                "fixedName passfix fixed@email.com"
            };
        }

        [Theory]
        [MemberData(nameof(EditSuccessfullyData))]
        public void EditSuccessfully(string[] args, string newName, string expected) {

            //arrange
            TestsHelper.SetUpDefault();

            //act
            var previous = getCommand.Run(new string[] { args[0] });
            editCommand.Run(args);
            var actual = getCommand.Run(new string[] { newName });

            //assert
            Assert.NotEqual(previous, actual);
            Assert.Equal(expected, actual.QueryReturnValue);

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

            yield return new object[] { EditCommand.InvalidSyntaxMessage, new string[] { validName, "newPassword" } };
            yield return new object[] { EditCommand.InvalidSyntaxMessage, new string[] { validName, "pass@eqwwr" } };

            yield return new object[] { EditCommand.InvalidKeyFound, new string[] { validName, "password:qrqrewqe", "nam:newname" } };
            yield return new object[] { EditCommand.InvalidKeyFound, new string[] { validName, "name:newname", "password:qweqwed" , "ema:email@thisone.com"} };

            yield return new object[] { EditCommand.DuplicateKeyFound, new string[] { validName, "email:email@somewhere.com", "email:someEma@here.com" } };
            yield return new object[] { EditCommand.DuplicateKeyFound, new string[] { validName, "password:newPassword1", "email:email@somewhere.com", "name:newName", "password:newvalidPassword" } };
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
