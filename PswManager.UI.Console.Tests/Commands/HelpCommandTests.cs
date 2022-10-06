using Moq;
using PswManager.Commands;
using PswManager.UI.Console.Commands;
using PswManager.UI.Console.Tests.Commands.Helper;

namespace PswManager.UI.Console.Tests.Commands;
public class HelpCommandTests {

    public HelpCommandTests() {
        Mock<ICommand> _mockedCommand = new();
        Mock<ICommand> _mockedTwoCommand = new();

        string mockedCommandDescription = "This command is being mocked and doesn't have any actual functionality.";

        _mockedCommand.Setup(x => x.GetDescription()).Returns(mockedCommandDescription);

        Dictionary<string, ICommand> commands = new();
        commands.Add("mocked", _mockedCommand.Object);

        helpCommand = new HelpCommand(commands);
        mockedCommand = _mockedCommand.Object;
        dicCommands = commands;
    }

    readonly ICommand mockedCommand;
    readonly IReadOnlyDictionary<string, ICommand> dicCommands;
    readonly HelpCommand helpCommand;

    public static IEnumerable<object[]> GetGenericHelpCorrectlyData() {
        static object[] NewObj(string val) => new object[] { val };

        yield return NewObj(null);
        yield return NewObj("");
        yield return NewObj("  ");
        yield return NewObj("    ");
    }

    [Theory]
    [MemberData(nameof(GetGenericHelpCorrectlyData))]
    public async Task GetGenericHelpCorrectly(string emptyValue) {

        //arrange
        string expectedToContain = string.Join("  ", dicCommands.Keys);
        var obj = ClassBuilder.Build<HelpCommand>(new List<string>() { emptyValue });

        //act
        var result = helpCommand.Run(obj);
        var resultAsync = await helpCommand.RunAsync(obj).ConfigureAwait(false);

        //assert
        Assert.True(result.Success);
        Assert.Contains(expectedToContain, result.QueryReturnValue);
        Assert.True(resultAsync.Success);
        Assert.Contains(expectedToContain, resultAsync.QueryReturnValue);

    }

    [Fact]
    public async Task GetSpecificCommandDescription() {

        //arrange
        string expectedDescription = mockedCommand.GetDescription();
        var obj = ClassBuilder.Build<HelpCommand>(new List<string>() { "mOcKed" });

        //act
        var result = helpCommand.Run(obj);
        var resultAsync = await helpCommand.RunAsync(obj).ConfigureAwait(false);

        //assert
        Assert.True(result.Success);
        Assert.Equal(expectedDescription, result.BackMessage);
        Assert.True(resultAsync.Success);
        Assert.Equal(expectedDescription, resultAsync.BackMessage);

    }

    [Fact]
    public async Task GetSyntaxButEmptyDictionary() {

        //arrange
        Dictionary<string, ICommand> commands = new();
        HelpCommand helpCommand = new(commands);
        string expected = "There has been an error: the command list is empty.";
        var obj = ClassBuilder.Build<HelpCommand>(new List<string>());

        //act
        var result = helpCommand.Run(obj);
        var resultAsync = await helpCommand.RunAsync(obj).ConfigureAwait(false);

        //assert
        Assert.False(result.Success);
        Assert.Equal(expected, result.BackMessage);
        Assert.False(resultAsync.Success);
        Assert.Equal(expected, resultAsync.BackMessage);

    }

    [Fact]
    public async Task Failure_GivenCommandDoesNotExist() {

        //arrange
        var args = ClassBuilder.Build<HelpCommand>(new List<string>() { "nonexistentcommand" });
        string expectedErrorMessage = HelpCommand.CommandInexistentErrorMessage;
        bool valid;
        CommandResult result;

        //act
        valid = helpCommand.Validate(args).success;
        result = helpCommand.Run(args);
        var resultAsync = await helpCommand.RunAsync(args).ConfigureAwait(false);

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

