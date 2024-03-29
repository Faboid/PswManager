﻿using PswManager.Commands;
using PswManager.TestUtils;
using PswManager.UI.Console.Tests.Commands.Helper;
using PswManager.UI.Console.Commands;
using PswManager.UI.Console.Inner;

namespace PswManager.UI.Console.Tests.Commands;
public class EditCommandTests {

    public EditCommandTests() {
        dbHandler = new MemoryDBHandler();
        dbHandler.SetUpDefaultValues();
        var dbFactory = dbHandler.GetDBFactory();
        editCommand = new EditCommand(new AccountEditor(dbFactory.GetDataEditor(), MockedObjects.GetEmptyCryptoAccount()));
        getCommand = new GetCommand(new AccountReader(dbFactory.GetDataReader(), MockedObjects.GetEmptyCryptoAccount()));
    }

    readonly GetCommand getCommand;
    readonly EditCommand editCommand;
    readonly MemoryDBHandler dbHandler;

    public static IEnumerable<object[]> EditSuccessfullyData() {
        static object[] NewObj(string name, string newName, string newPassword, string newEmail, string expected)
            => new object[] {
                name,
                newName ?? name,
                ClassBuilder.Build<EditCommand>(new List<string> { newPassword, newName, newEmail, name }),
                expected
            };

        var def = new DefaultValues(4);

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
        dbHandler.SetUpDefaultValues();

        //act
        var previous = getCommand.Run(ClassBuilder.Build<GetCommand>(new List<string>() { name }));
        var result = editCommand.Run(args);
        var actual = getCommand.Run(ClassBuilder.Build<GetCommand>(new List<string>() { newName ?? name }));

        //assert
        Assert.NotEqual(previous, actual);
        Assert.Equal(expected, actual.QueryReturnValue);
        Assert.True(result.Success);

    }

    [Theory]
    [MemberData(nameof(EditSuccessfullyData))]
    public async Task EditSuccessfullyAsync(string name, string newName, ICommandInput args, string expected) {

        //arrange
        dbHandler.SetUpDefaultValues();

        //act
        var previous = await getCommand.RunAsync(ClassBuilder.Build<GetCommand>(new List<string>() { name })).ConfigureAwait(false);
        var result = await editCommand.RunAsync(args).ConfigureAwait(false);
        var actual = await getCommand.RunAsync(ClassBuilder.Build<GetCommand>(new List<string>() { newName ?? name })).ConfigureAwait(false);

        //assert
        Assert.NotEqual(previous, actual);
        Assert.Equal(expected, actual.QueryReturnValue);
        Assert.True(result.Success);

    }

    public static IEnumerable<object[]> ExpectedValidationFailuresData() {
        static object[] NewObj(string errorMessage, string name, string newName, string newPassword, string newEmail)
            => new object[] {
                errorMessage,
                ClassBuilder.Build<EditCommand>(new List < string > { newPassword, newName, newEmail, name })
            };


        yield return NewObj(ErrorReader.GetRequiredError<EditCommand>("Name"), null, "someValue", "hrhr", "");
    }

    [Theory]
    [MemberData(nameof(ExpectedValidationFailuresData))]
    public async void ExpectedValidationFailures(string expectedErrorMessage, ICommandInput args) {

        //arrange
        bool valid;
        CommandResult result;
        CommandResult resultAsync;

        //act
        valid = editCommand.Validate(args).success;
        result = editCommand.Run(args);
        resultAsync = await editCommand.RunAsync(args).ConfigureAwait(false);

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
