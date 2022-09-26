using PswManager.Commands;
using PswManager.ConsoleUI.Commands;
using PswManager.ConsoleUI.Inner;
using PswManager.ConsoleUI.Tests.Commands.Helper;
using PswManager.TestUtils;

namespace PswManager.ConsoleUI.Tests.Commands;
public class GetCommandTests {

    public GetCommandTests() {
        var dbFactory = new MemoryDBHandler(1).SetUpDefaultValues().GetDBFactory();
        getCommand = new GetCommand(new AccountReader(dbFactory.GetDataReader(), MockedObjects.GetEmptyCryptoAccount()));
    }

    readonly GetCommand getCommand;

    [Fact]
    public async Task CommandSuccess() {

        //arrange
        CommandResult result;
        CommandResult resultAsync;
        string expected = new DefaultValues(1).values[0];
        var obj = ClassBuilder.Build<GetCommand>(new List<string>() { DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name) });

        //act
        result = getCommand.Run(obj);
        resultAsync = await getCommand.RunAsync(obj).ConfigureAwait(false);

        //assert
        Assert.True(result.Success);
        Assert.True(resultAsync.Success);
        Assert.Equal(expected, result.QueryReturnValue);
        Assert.Equal(expected, resultAsync.QueryReturnValue);

    }

    public static IEnumerable<object[]> ExpectedValidationFailuresData() {
        static object[] NewObj(string errorMessage, string name)
            => new object[] {
                errorMessage,
                ClassBuilder.Build<GetCommand>(new List<string> { name })
            };

        string missingNameMessage = ErrorReader.GetRequiredError<GetCommand>("Name");

        yield return NewObj(missingNameMessage, "");
        yield return NewObj(missingNameMessage, null);
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
        valid = getCommand.Validate(args).success;
        result = getCommand.Run(args);
        resultAsync = await getCommand.RunAsync(args).ConfigureAwait(false);

        //assert
        Assert.False(valid);

        //sync
        Assert.False(result.Success);
        Assert.NotEmpty(result.ErrorMessages);
        Assert.Contains(expectedErrorMessage, result.ErrorMessages);

        //async
        Assert.False(resultAsync.Success);
        Assert.NotEmpty(resultAsync.ErrorMessages);
        Assert.Contains(expectedErrorMessage, resultAsync.ErrorMessages);

    }

}
