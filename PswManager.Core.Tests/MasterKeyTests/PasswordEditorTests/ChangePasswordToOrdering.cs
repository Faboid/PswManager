using PswManager.Core.AccountModels;
using PswManager.Core.MasterKey;
using PswManager.Core.Services;
using PswManager.Database;
using PswManager.TestUtils;
using static PswManager.Core.MasterKey.PasswordStatusChecker;

namespace PswManager.Core.Tests.MasterKeyTests.PasswordEditorTests;

public class ChangePasswordToOrdering {

    public ChangePasswordToOrdering() {
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
    }

    [Fact]
    public async Task HappyPathOrder() {

        var orderChecker = new OrderChecker();
        ResetMocks();

        _passwordStatusCheckerMock.Setup(x => x.SetStatusTo(PasswordStatus.Starting)).Callback(() => orderChecker.Done(1));
        _bufferHandlerMock.Setup(x => x.Backup()).Callback(() => orderChecker.Done(2, 3));
        _accountsHandlerMock.Setup(x => x.SetupAccounts(It.IsAny<IAccountModelFactory>()))
            .Returns(() => Task.FromResult(_accountsHandlerExecutableMock.Object))
            .Callback(() => orderChecker.Done(2, 3));

        _passwordStatusCheckerMock.Setup(x => x.SetStatusTo(PasswordStatus.Pending)).Callback(() => orderChecker.Done(4));
        _accountsHandlerExecutableMock.Setup(x => x.ExecuteUpdate()).Callback(() => orderChecker.Done(5, 6));
        _cryptoAccountServiceFactoryMock.Setup(x => x.SignUpAccountAsync(It.IsAny<char[]>())).Callback(() => orderChecker.Done(5, 6));

        _bufferHandlerMock.Setup(x => x.Free()).Callback(() => orderChecker.Done(7, 9));
        _passwordStatusCheckerMock.Setup(x => x.Free()).Callback(() => orderChecker.Done(7, 9));
        _accountsHandlerMock.Setup(x => x.UpdateModelFactory(It.IsAny<IAccountModelFactory>())).Callback(() => orderChecker.Done(7, 9));

        var result = await _sut.ChangePasswordTo("SomePassword".ToCharArray());
        orderChecker.Done(10);

        Assert.Equal(PasswordChangeResult.Success, result);

    }

    [Fact]
    public async Task BadPathOrder() {

        var orderChecker = new OrderChecker();
        ResetMocks();

        _passwordStatusCheckerMock.Setup(x => x.SetStatusTo(PasswordStatus.Starting)).Callback(() => orderChecker.Done(1));
        _bufferHandlerMock.Setup(x => x.Backup()).Callback(() => orderChecker.Done(2, 3));
        _accountsHandlerMock.Setup(x => x.SetupAccounts(It.IsAny<IAccountModelFactory>()))
            .Returns(() => Task.FromResult(_accountsHandlerExecutableMock.Object))
            .Callback(() => orderChecker.Done(2, 3));

        _passwordStatusCheckerMock.Setup(x => x.SetStatusTo(PasswordStatus.Pending)).Callback(() => orderChecker.Done(4));
        _accountsHandlerExecutableMock.Setup(x => x.ExecuteUpdate()).Callback(() => orderChecker.Done(5, 6));
        _cryptoAccountServiceFactoryMock.Setup(x => x.SignUpAccountAsync(It.IsAny<char[]>()))
            .ThrowsAsync(new Exception())
            .Callback(() => orderChecker.Done(5, 6));

        _passwordStatusCheckerMock.Setup(x => x.SetStatusTo(PasswordStatus.Failed)).Callback(() => orderChecker.Done(7));
        _bufferHandlerMock.Setup(x => x.Restore()).Callback(() => orderChecker.Done(8));

        _bufferHandlerMock.Setup(x => x.Free()).Callback(() => orderChecker.Done(9, 10));
        _passwordStatusCheckerMock.Setup(x => x.Free()).Callback(() => orderChecker.Done(9, 10));
        _accountsHandlerMock.Setup(x => x.UpdateModelFactory(It.IsAny<IAccountModelFactory>())).Callback(() => OrderChecker.Never());

        var result = await _sut.ChangePasswordTo("SomePassword".ToCharArray());
        orderChecker.Done(11);

        Assert.Equal(PasswordChangeResult.Failure, result);

    }

}