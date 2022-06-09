﻿using PswManager.Commands;
using PswManager.Database.DataAccess.Interfaces;
using Xunit;
using PswManager.ConsoleUI.Tests.Commands.Helper;
using PswManager.ConsoleUI.Commands;
using PswManager.TestUtils;
using PswManager.Core.Inner;

namespace PswManager.ConsoleUI.Tests.Commands {

    public class DeleteCommandTests {

        public DeleteCommandTests() {
            var dbFactory = new MemoryDBHandler(2).SetUpDefaultValues().GetDBFactory();
            delCommand = new DeleteCommand(new AccountDeleter(dbFactory.GetDataDeleter()), MockedObjects.GetDefaultAutoInput());
            dataHelper = dbFactory.GetDataHelper();
        }

        readonly IDataHelper dataHelper;
        readonly DeleteCommand delCommand;

        [Fact]
        public void DeleteSuccessfully() {

            //arrange
            string name = DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name);
            var obj = ClassBuilder.Build<DeleteCommand>(new List<string> { name });

            //act
            bool exist = dataHelper.AccountExist(name);
            delCommand.Run(obj);

            //assert
            Assert.True(exist);
            Assert.False(dataHelper.AccountExist(name));

        }

        [Fact]
        public async Task DeleteSuccessfullyAsync() {

            //arrange
            string name = DefaultValues.StaticGetValue(1, DefaultValues.TypeValue.Name);
            var obj = ClassBuilder.Build<DeleteCommand>(new List<string> { name });

            //act
            bool exist = await dataHelper.AccountExistAsync(name).ConfigureAwait(false);
            await delCommand.RunAsync(obj).ConfigureAwait(false);

            //assert
            Assert.True(exist);
            Assert.False(await dataHelper.AccountExistAsync(name).ConfigureAwait(false));

        }

        public static IEnumerable<object[]> ExpectedValidationFailuresData() {
            static object[] NewObj(string errorMessage, string name)
                => new object[] {
                    errorMessage,
                    ClassBuilder.Build<DeleteCommand>(new List<string> { name })
                };

            string missingNameMessage = ErrorReader.GetRequiredError<DeleteCommand>("Name");

            yield return NewObj(missingNameMessage, null);
            yield return NewObj(missingNameMessage, "");
            //todo - yield return NewObj(missingNameMessage, "   ");

        }

        [Theory]
        [MemberData(nameof(ExpectedValidationFailuresData))]
        public async Task ExpectedValidationFailures(string expectedErrorMessage, ICommandInput args) {

            //arrange
            bool valid;
            CommandResult result;
            CommandResult resultAsync;

            //act
            valid = delCommand.Validate(args).success;
            result = delCommand.Run(args);
            resultAsync = await delCommand.RunAsync(args).ConfigureAwait(false);

            //assert
            Assert.False(valid);

            //sync result
            Assert.False(result.Success);
            Assert.NotEmpty(result.ErrorMessages);
            Assert.Contains(expectedErrorMessage, result.ErrorMessages);

            //async result
            Assert.False(resultAsync.Success);
            Assert.NotEmpty(resultAsync.ErrorMessages);
            Assert.Contains(expectedErrorMessage, resultAsync.ErrorMessages);

        }

    }
}
