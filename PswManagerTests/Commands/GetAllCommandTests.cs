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
            getAllCommand = new GetAllCommand(dataFactory.GetDataReader(), TestsHelper.CryptoAccount);
        }

        readonly GetAllCommand getAllCommand;

        public static IEnumerable<object[]> CommandSuccessData() {
            static object[] NewObject(IEnumerable<string> expectedValues, params string[] input) => new object[] { input, expectedValues.ToArray() };
            var dict = new Dictionary<int, DefaultValues.TypeValue> {
                { 0, DefaultValues.TypeValue.Name },
                { 1, DefaultValues.TypeValue.Password },
                { 2, DefaultValues.TypeValue.Email }
            };
            static string GetVal(int index, DefaultValues.TypeValue type) => TestsHelper.DefaultValues.GetValue(index, type);
            IEnumerable<int> accountsPositions = Enumerable.Range(0, TestsHelper.DefaultValues.values.Count);

            //test return of all single values
            yield return NewObject(accountsPositions.Select(x => GetVal(x, dict[0])), "names");
            yield return NewObject(accountsPositions.Select(x => GetVal(x, dict[1])), "passwords");
            yield return NewObject(accountsPositions.Select(x => GetVal(x, dict[2])), "emails");

            //tests combinations
            yield return NewObject(accountsPositions.Select(x => $"{GetVal(x, dict[0])} {GetVal(x, dict[1])}"), "names", "passwords");
            yield return NewObject(accountsPositions.Select(x => $"{GetVal(x, dict[1])} {GetVal(x, dict[2])}"), "passwords", "emails");
            yield return NewObject(accountsPositions.Select(x => $"{GetVal(x, dict[0])} {GetVal(x, dict[2])}"), "names", "emails");

            //tests getting all
            yield return NewObject(accountsPositions.Select(x => TestsHelper.DefaultValues.values[x]));
            yield return NewObject(accountsPositions.Select(x => TestsHelper.DefaultValues.values[x]), "names", "passwords", "emails");
        }

        [Theory]
        [MemberData(nameof(CommandSuccessData))]
        public void CommandSuccess(string[] input, string[] expectedValues) {

            //arrange
            TestsHelper.SetUpDefault();
            CommandResult result;

            //act
            result = getAllCommand.Run(input);

            //assert
            Assert.Equal(string.Join(Environment.NewLine, expectedValues), result.QueryReturnValue);

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {

            yield return new object[] { ValidationCollection.ArgumentsNullMessage, null };
            yield return new object[] { ValidationCollection.WrongArgumentsNumberMessage, new string[] { "defaultName1", "tirhtewygh", "email@somewhere.com", "something" } };

            yield return new object[] { GetAllCommand.InexistentKeyErrorMessage, new string[] { "fakeKey", "names" } };
            yield return new object[] { GetAllCommand.DuplicateKeyErrorMessage, new string[] { "names", "names", "passwords" } };
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
