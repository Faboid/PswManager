using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase;
using PswManagerLibrary.Commands.ManualCommands;
using PswManagerLibrary.Extensions;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Commands {

    [Collection("TestHelperCollection")]
    public class GetCommandTests {

        public GetCommandTests() {
            IDataFactory dataFactory = new DataFactory(TestsHelper.Paths);
            getCommand = new GetCommand(dataFactory.GetDataReader(), TestsHelper.CryptoAccount);
        }

        readonly GetCommand getCommand;

        [Fact]
        public void CommandSuccess() {

            //arrange
            CommandResult result;

            //act
            result = getCommand.Run(new string[] { TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name) });

            //assert
            Assert.Equal(TestsHelper.DefaultValues.values[0], result.QueryReturnValue);

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
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "tirhtewygh", "email@somewhere.com" } };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { validName, "tirhtewygh", "email@somewhere.com", "something" } };
        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, params string[] args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = getCommand.Validate(args).success;
            result = getCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
