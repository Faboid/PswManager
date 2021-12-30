using PswManagerCommands;
using PswManagerLibrary.Commands;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Commands {

    [Collection("TestHelperCollection")]
    public class GetCommandTests {

        readonly GetCommand getCommand;

        public GetCommandTests() {
            getCommand = new(TestsHelper.PswManager);
        }

        [Fact]
        public void CommandSuccess() {

            //arrange
            CommandResult result;

            //act
            result = getCommand.Run(new string[] { TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name) });

            //assert
            Assert.Equal(TestsHelper.DefaultValues.values[0], result.QueryReturnValue);

        }

        [Fact]
        public void CommandFailure_AccountDoesNotExist() {

            //arrange
            TestsHelper.SetUpDefault();
            var args = new string[] { "notexistentname" };

            //act & assert
            Failure_GenericTestType(args);

        }

        public static IEnumerable<object[]> NullArgumentsData() {
            yield return new object[] { null };
            yield return new object[] { new string[] { null } };
        }

        [Theory]
        [MemberData(nameof(NullArgumentsData))]
        public void CommandFailure_NullArguments(string[] args) {

            //assert & act
            Failure_GenericTestType(args);

        }

        public static IEnumerable<object[]> IncorrectNumberArgumentsData() {
            yield return new object[] { new string[] { "validName", "validEmail@emaildomain.com" } };
            yield return new object[] { new string[] { "justAName", "thisPassword", "validEmail@emaildomain.com" } };
            yield return new object[] { Array.Empty<string>() };
            yield return new object[] { new string[] { "firstName", "secondName", "passwordhere", "validEmail@emaildomain.com" } };
        }

        [Theory]
        [MemberData(nameof(IncorrectNumberArgumentsData))]
        public void CommandFailure_IncorrectNumberArguments(string[] args) {

            //act & assert
            Failure_GenericTestType(args);

        }

        [Fact]
        public void CommandFailure_EmptyName() {

            //arrange
            var args = new string[] { "" };

            //act & assert
            Failure_GenericTestType(args);
        }

        private void Failure_GenericTestType(string[] args) {
            bool valid;
            CommandResult result;

            //act
            valid = getCommand.Validate(args).success;
            result = getCommand.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
        }
    }
}
