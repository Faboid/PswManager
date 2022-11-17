using PswManager.Core.AccountModels;
using PswManager.Core.MasterKey;
using PswManager.Core.Services;
using PswManager.Database;

namespace PswManager.Core.Tests.MasterKeyTests.PasswordEditorTests;

public class ErrorHandling {

    public ErrorHandling() {
        _sut = new(_bufferHandlerMock.Object, _passwordStatusCheckerMock.Object, _accountsHandlerMock.Object, _cryptoAccountServiceFactoryMock.Object);
    }

    private readonly Mock<ICryptoAccountServiceFactory> _cryptoAccountServiceFactoryMock = new();
    private readonly Mock<IDataConnection> _dataConnectionMock = new();
    private readonly Mock<IAccountsHandler> _accountsHandlerMock = new();
    private readonly Mock<IAccountsHandlerExecutable> _accountsHandlerExecutableMock = new();
    private readonly Mock<IBufferHandler> _bufferHandlerMock = new();
    private readonly Mock<IPasswordStatusChecker> _passwordStatusCheckerMock = new();
    private readonly PasswordEditor _sut;

    private void ResetMocks() {
        _cryptoAccountServiceFactoryMock.Reset();
        _dataConnectionMock.Reset();
        _bufferHandlerMock.Reset();
        _passwordStatusCheckerMock.Reset();
        _accountsHandlerMock.Setup(x => x.SetupAccounts(It.IsAny<IAccountModelFactory>())).Returns(() => Task.FromResult(_accountsHandlerExecutableMock.Object));
    }

    [Fact]
    public async Task ThrowsIfDuringStarting() {

        ResetMocks();
        _bufferHandlerMock.Setup(x => x.Backup()).Throws(new InvalidDataException());

        await Assert.ThrowsAsync<InvalidDataException>(() => _sut.ChangePasswordTo("Something"));

    }

    [Fact]
    public async Task HandledIfThrowsDuringPending() {

        ResetMocks();
        _accountsHandlerExecutableMock.Setup(x => x.ExecuteUpdate()).Throws(new InvalidCastException());

        var result = await _sut.ChangePasswordTo("Something");
        Assert.Equal(PasswordChangeResult.Failure, result);

    }

    [Fact]
    public async Task ThrowsIfTheRestoreThrows() {

        ResetMocks();
        _accountsHandlerExecutableMock.Setup(x => x.ExecuteUpdate()).Throws(new InvalidOperationException());
        _bufferHandlerMock.Setup(x => x.Restore()).Throws(new InvalidDataException());

        await Assert.ThrowsAsync<InvalidDataException>(() => _sut.ChangePasswordTo("Something"));

    }

}