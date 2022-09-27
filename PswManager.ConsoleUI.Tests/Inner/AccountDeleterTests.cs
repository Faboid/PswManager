using Moq;
using PswManager.ConsoleUI.Inner;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Extensions;
using PswManager.TestUtils;
using PswManager.Utils;
using PswManager.Utils.Options;

namespace PswManager.ConsoleUI.Tests.Inner;
public class AccountDeleterTests {

    public AccountDeleterTests() {

        dataDeleterMock = new();
        dataDeleterMock
            .Setup(x => x.DeleteAccountAsync(It.IsAny<string>()))
            .Returns<string>(x => (string.IsNullOrWhiteSpace(x) ? DeleterResponseCode.InvalidName : DeleterResponseCode.Success).AsTask());
    }

    readonly Mock<IDataDeleter> dataDeleterMock;

    [Theory]
    [InlineData("someName")]
    public async Task DeleteAccountAsyncCallsDBCorrectly(string name) {

        //arrange
        AccountDeleter deleter = new(dataDeleterMock.Object);

        //act
        var result = await deleter.DeleteAccountAsync(name);

        //assert
        Assert.Equal(DeleterResponseCode.Success, result);
        dataDeleterMock.Verify(x => x.DeleteAccountAsync(It.Is<string>(x => x == name)));
        dataDeleterMock.VerifyNoOtherCalls();

    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task NoCallsIfInvalidName(string name) {

        //arrange
        AccountDeleter creator = new(dataDeleterMock.Object);

        //act
        var result = creator.DeleteAccount(name);
        var resultAsync = await creator.DeleteAccountAsync(name);

        //assert
        Assert.Equal(DeleterResponseCode.InvalidName, result);
        Assert.Equal(DeleterResponseCode.InvalidName, resultAsync);
        dataDeleterMock.VerifyNoOtherCalls();

    }

}
