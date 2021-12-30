using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerTests.TestsHelpers;
using PswManagerLibrary.Extensions;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace PswManagerTests.Commands {

    //todo - either implement remaining tests or find a cleaner way of handling generic tests

    public class CommandTestsHelper {

        public CommandTestsHelper(BaseCommand command, string[] accountExistenceArguments, int minArgsNumber, int maxArgsNumber) {
            Command = command;
            AccountExistenceArguments = accountExistenceArguments;
            MinArgsNumber = minArgsNumber;
            MaxArgsNumber = maxArgsNumber;
        }


        public BaseCommand Command { get; }
        public string[] AccountExistenceArguments { get; }
        public IEnumerable<object[]> NullArgumentsData { get; }
        public bool ExpectAccountExistance { get; }
        public int MinArgsNumber { get; }
        public int MaxArgsNumber { get; }
        public string[] ValidArguments { get; }
    }

    [Collection("TestHelperCollection")]
    public abstract class BaseCommandTests {

        public BaseCommandTests(ITestOutputHelper output) {
            _output = output;
        }

        private ITestOutputHelper _output;
        protected abstract CommandTestsHelper GetHelper();

        /// <summary>
        /// Some commands require the account to exist, while others the reverse. Using <see cref="ExpectAccountExistance"/>, this is written in a generic manner to be used on both cases.
        /// </summary>
        [Fact]
        public void CommandFailure_Existence() {

            if(GetHelper().ExpectAccountExistance is false) {
                _output.WriteLine("Test skipped because this command does not require the existence of the account.");
                return;
            }

            //arrange
            TestsHelper.SetUpDefault();
            var args = GetHelper().AccountExistenceArguments;

            //act & assert
            Failure_GenericTestType(args, new ValidationCollection(null).InexistentAccountMessage());

        }

        public static IEnumerable<object[]> NullArgumentsData() {
            yield return new object[] { null };
            yield return new object[] { new string[] { null } };
            yield return new object[] { new string[] { null, "&&%£@#[][+*é", "valueNameHere@thisdomain.com" } };
            yield return new object[] { new string[] { "valuenare", null, "valueNameHere@thisdomain.com" } };
            yield return new object[] { new string[] { "justvalue##", "riewhguyrui", null } };
        }

        [Theory]
        [MemberData(nameof(NullArgumentsData))]
        public void CommandFailure_NullArguments(string[] args) {

            if(args?.Length < GetHelper().MinArgsNumber || args?.Length > GetHelper().MaxArgsNumber) {
                //todo - find a less messy way of handling this.
                //since this test checks only for null arguments, it shouldn't go on in case the arguments are faulty for another reason
                _output.WriteLine("Skipped test.");
                return;
            }


            Failure_GenericTestType(args, ValidationCollection.ArgumentsNullOrEmptyMessage);

        }

        protected void Failure_GenericTestType(string[] args, string necessaryErrorMessage) {

            //arrange
            bool valid;
            CommandResult result;

            //act
            valid = GetHelper().Command.Validate(args).success;
            result = GetHelper().Command.Run(args);

            //assert
            Assert.False(valid);
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(necessaryErrorMessage, result.ErrorMessages);
        }

    }
}
