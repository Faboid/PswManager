using PswManagerCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase;
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
    public class GetAllCommandTests {

        public GetAllCommandTests() {
            IDataFactory dataFactory = new DataFactory(TestsHelper.Paths);
            getAllCommand = new GetAllCommand(dataFactory.GetDataReader());
        }

        readonly GetAllCommand getAllCommand;

        [Fact]
        public void CommandSuccess() {

            //arrange
            TestsHelper.SetUpDefault();
            CommandResult result;

            //act
            result = getAllCommand.Run(new string[] {});
            var defNames = TestsHelper.DefaultValues.values
                .Select(x => x.Split(' ').First());

            //assert
            Assert.Equal(string.Join(' ', defNames), result.QueryReturnValue);

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {

            yield return new object[] { ValidationCollection.ArgumentsNullMessage, null };

            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { "something" } };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { "Someval", "eiwghrywhgi" } };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { "aName", "tirhtewygh", "email@somewhere.com" } };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { "defaultName1", "tirhtewygh", "email@somewhere.com", "something" } };
        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, params string[] args) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = getAllCommand.Validate(args).success;
            result = getAllCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
