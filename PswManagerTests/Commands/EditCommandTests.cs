using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Commands.Validation.Attributes;
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
                    ClassBuilder.Build<EditCommand>(new List<string> { newPassword, newName, newEmail, name }),
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
                    ClassBuilder.Build<EditCommand>(new List < string > { newPassword, newName, newEmail, name })
                };

            string validName = TestsHelper.DefaultValues.GetValue(3, DefaultValues.TypeValue.Name);
            string validName2 = TestsHelper.DefaultValues.GetValue(4, DefaultValues.TypeValue.Name);

            yield return NewObj(ErrorReader.GetRequiredError<EditCommand>("Name"), null, "someValue", "hrhr", "");
            yield return NewObj(ErrorReader.GetError<EditCommand, VerifyAccountExistenceAttribute>("Name"), "fakeAccountName", null, "newPasshere", null);
            yield return NewObj(ErrorReader.GetError<EditCommand, VerifyAccountExistenceAttribute>("NewName"), validName, validName2, null, null);
            
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
