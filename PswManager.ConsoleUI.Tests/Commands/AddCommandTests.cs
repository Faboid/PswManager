using PswManager.Commands;
using PswManager.ConsoleUI.Tests.Commands.Helper;
using PswManager.ConsoleUI.Commands;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.ConsoleUI.Inner;
using PswManager.Database.Interfaces;

namespace PswManager.ConsoleUI.Tests.Commands;
public class AddCommandTests {

    public AddCommandTests() {
        var dbFactory = new MemoryDBHandler(1).SetUpDefaultValues().GetDBFactory();
        AccountCreator creator = new(dbFactory.GetDataCreator(), MockedObjects.GetEmptyCryptoAccount());
        addCommand = new AddCommand(creator);
        dataHelper = dbFactory.GetDataHelper();
    }

    readonly IDataHelper dataHelper;
    readonly ICommand addCommand;

    [Theory]
    [InlineData("justSomeName#9839", "random@#[ssword", "random@email.it")]
    [InlineData("xmlnyyx", "ightueghtuy", "this@mail.com")]
    [InlineData("valueNamehere", "&&%£@#[][+*é", "valueNameHere@thisdomain.com")]
    public void AddSuccessfully(string name, string password, string email) {

        //arrange
        var obj = ClassBuilder.Build<AddCommand>(new List<string> { password, name, email});
        CommandResult result;

        //act
        var exists = dataHelper.AccountExist(name);
        result = addCommand.Run(obj);

        //assert
        Assert.Equal(AccountExistsStatus.NotExist, exists);
        Assert.True(result.Success);
        Assert.Equal(AccountExistsStatus.Exist, dataHelper.AccountExist(name));

    }

    [Theory]
    [InlineData("someasyncname", "passhere", "ema@yoyo,com")]
    [InlineData("xmlnyyasyncx", "ightueasyncghtuy", "this@mail.com")]
    [InlineData("AsyncValuehere", "&&%£@#[][+*é", "valueNameHere@thisdomain.com")]
    public async Task AddSuccessfullyAsync(string name, string password, string email) {

        //arrange
        var obj = ClassBuilder.Build<AddCommand>(new List<string> { password, name, email });
        CommandResult result;

        //act
        var exists = await dataHelper.AccountExistAsync(name).ConfigureAwait(false);
        result = await addCommand.RunAsync(obj).ConfigureAwait(false);

        //assert
        Assert.Equal(AccountExistsStatus.NotExist, exists);
        Assert.True(result.Success);
        Assert.Equal(AccountExistsStatus.Exist, await dataHelper.AccountExistAsync(name).ConfigureAwait(false));

    }

    public static IEnumerable<object[]> ExpectedValidationFailuresData() {
        static object[] NewObj(string errorMessage, string name, string password, string email) 
            => new object[] { 
                errorMessage, 
                ClassBuilder.Build<AddCommand>(new List<string> { password, name, email}) 
            };

        string validName = "someRandomNonexistentAccountName";

        //check for empty/null values
        yield return NewObj(ErrorReader.GetRequiredError<AddCommand>("Name"), "", null, "email@here.com");
        yield return NewObj(ErrorReader.GetRequiredError<AddCommand>("Name"), null, "somepass", "email@here.com");
        yield return NewObj(ErrorReader.GetRequiredError<AddCommand>("Password"), validName, null, "email@here.com");
        yield return NewObj(ErrorReader.GetRequiredError<AddCommand>("Email"), validName, "rightuewih", "");
    }

    [Theory]
    [MemberData(nameof(ExpectedValidationFailuresData))]
    public async void ExpectedValidationFailures(string expectedErrorMessage, ICommandInput args) {

        //arrange
        bool valid;
        CommandResult result;
        CommandResult resultAsync;

        //act
        valid = addCommand.Validate(args).success;
        result = addCommand.Run(args);
        resultAsync = await addCommand.RunAsync(args).ConfigureAwait(false);

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
