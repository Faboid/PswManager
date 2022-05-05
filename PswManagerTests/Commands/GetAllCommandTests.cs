﻿using PswManagerCommands;
using PswManagerLibrary.Commands;
using PswManagerTests.Commands.Helper;
using PswManagerTests.Database.MemoryConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PswManagerTests.Commands {
    public class GetAllCommandTests {

        public GetAllCommandTests() {
            var dbFactory = new MemoryDBHandler(numValues).SetUpDefaultValues().GetDBFactory();
            getAllCommand = new GetAllCommand(dbFactory.GetDataReader(), MockedObjects.GetEmptyCryptoAccount());
        }

        const int numValues = 5;
        readonly GetAllCommand getAllCommand;

        public static IEnumerable<object[]> CommandSuccessData() {
            static object[] NewObject(IEnumerable<string> expectedValues, params string[] input) => new object[] { string.Join(' ', input), expectedValues.ToArray() };
            var dict = new Dictionary<int, DefaultValues.TypeValue> {
                { 0, DefaultValues.TypeValue.Name },
                { 1, DefaultValues.TypeValue.Password },
                { 2, DefaultValues.TypeValue.Email }
            };
            IEnumerable<int> accountsPositions = Enumerable.Range(0, numValues);
            var defValues = new DefaultValues(numValues);
            string GetVal(int index, DefaultValues.TypeValue type) => defValues.GetValue(index, type);

            //test return of all single values
            yield return NewObject(accountsPositions.Select(x => GetVal(x, dict[0])), "names");
            yield return NewObject(accountsPositions.Select(x => GetVal(x, dict[1])), "passwords");
            yield return NewObject(accountsPositions.Select(x => GetVal(x, dict[2])), "emails");

            //tests combinations
            yield return NewObject(accountsPositions.Select(x => $"{GetVal(x, dict[0])} {GetVal(x, dict[1])}"), "names", "passwords");
            yield return NewObject(accountsPositions.Select(x => $"{GetVal(x, dict[1])} {GetVal(x, dict[2])}"), "passwords", "emails");
            yield return NewObject(accountsPositions.Select(x => $"{GetVal(x, dict[0])} {GetVal(x, dict[2])}"), "names", "emails");

            //tests getting all
            yield return NewObject(accountsPositions.Select(x => defValues.values[x]));
            yield return NewObject(accountsPositions.Select(x => defValues.values[x]), "names", "passwords", "emails");
            yield return new object[] { null, accountsPositions.Select(x => defValues.values[x]) };
        }

        [Theory]
        [MemberData(nameof(CommandSuccessData))]
        public void CommandSuccess(string input, string[] expectedValues) {

            //arrange
            CommandResult result;
            var obj = ClassBuilder.Build<GetAllCommand>(new List<string>() { input });

            //act
            result = getAllCommand.Run(obj);

            //assert
            foreach(var value in expectedValues) {
                Assert.Contains(value, result.QueryReturnValue);
            }

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            static object[] NewObj(string errorMessage, params string[] keys) => new object[] { errorMessage, string.Join(' ', keys) };

            yield return NewObj(GetAllCommand.InexistentKeyErrorMessage, "fakeKey", "names");
            yield return NewObj(GetAllCommand.DuplicateKeyErrorMessage, "names", "names", "passwords");

        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public void ExpectedValidationFailures(string expectedErrorMessage, string keys) {

            //arrange
            bool valid;
            CommandResult result;
            var obj = ClassBuilder.Build<GetAllCommand>(new List<string>() { keys });

            //act
            valid = getAllCommand.Validate(obj).success;
            result = getAllCommand.Run(obj);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        }

    }
}
