using PswManager.Core.MasterKey;
using PswManager.Core.Services;
using PswManager.Database;
using PswManager.TestUtils;
using static PswManager.Core.MasterKey.PasswordStatusChecker;

namespace PswManager.Core.Tests.MasterKeyTests.PasswordEditorFactoryTests;

public class StartupCheck {

    public StartupCheck() {
        _sut = new(_bufferHandlerMock.Object, _passwordStatusCheckerMock.Object, _cryptoAccountServiceFactoryMock.Object, Mock.Of<IDataConnection>());
    }

    private readonly Mock<ICryptoAccountServiceFactory> _cryptoAccountServiceFactoryMock = new();
    private readonly Mock<IBufferHandler> _bufferHandlerMock = new();
    private readonly Mock<IPasswordStatusChecker> _passwordStatusCheckerMock = new();
    private readonly PasswordEditorFactory _sut;

    private void ResetMocks() {
        _cryptoAccountServiceFactoryMock.Reset();
        _bufferHandlerMock.Reset();
        _passwordStatusCheckerMock.Reset();
    }

    [Theory]
    [InlineData(PasswordStatus.Pending)]
    [InlineData(PasswordStatus.Failed)]
    internal async Task IfPendingOrFailedRestoreBuffer(PasswordStatus status) {

        ResetMocks();
        _bufferHandlerMock.Setup(x => x.Exists).Returns(true);
        _passwordStatusCheckerMock.Setup(x => x.GetStatus()).Returns(Task.FromResult(status));

        await _sut.StartupCheckup();

        _bufferHandlerMock.Verify(x => x.Restore());
        _bufferHandlerMock.Verify(x => x.Free());
        _passwordStatusCheckerMock.Verify(x => x.Free());

    }

    [Theory]
    [InlineData(PasswordStatus.Starting)]
    [InlineData(PasswordStatus.Unknown)]
    [InlineData(PasswordStatus.None)]
    internal async Task NotPendingDoesNotRestore(PasswordStatus status) {

        ResetMocks();
        _bufferHandlerMock.Setup(x => x.Exists).Returns(true);
        _passwordStatusCheckerMock.Setup(x => x.GetStatus()).Returns(Task.FromResult(status));

        await _sut.StartupCheckup();

        _bufferHandlerMock.Verify(x => x.Restore(), Times.Never());
        _bufferHandlerMock.Verify(x => x.Free(), Times.Never());
        _passwordStatusCheckerMock.Verify(x => x.Free(), Times.Never());

    }

    [Fact]
    public async Task MissingBufferDoesNotRestore() {

        ResetMocks();
        _bufferHandlerMock.Setup(x => x.Exists).Returns(false);

        await _sut.StartupCheckup();

        _bufferHandlerMock.Verify(x => x.Restore(), Times.Never());
        _bufferHandlerMock.Verify(x => x.Free(), Times.Never());
        _passwordStatusCheckerMock.Verify(x => x.Free(), Times.Never());
        _passwordStatusCheckerMock.Verify(x => x.GetStatus(), Times.Never());

    }

    [Fact]
    internal async Task ResourcesAreFreedLast() {

        var orderChecker = new OrderChecker();
        ResetMocks();
        _bufferHandlerMock.Setup(x => x.Exists).Returns(true);
        _passwordStatusCheckerMock.Setup(x => x.GetStatus()).Returns(Task.FromResult(PasswordStatus.Pending));

        _bufferHandlerMock.Setup(x => x.Restore()).Callback(() => orderChecker.Done(1));
        _bufferHandlerMock.Setup(x => x.Free()).Callback(() => orderChecker.Done(2));
        _passwordStatusCheckerMock.Setup(x => x.Free()).Callback(() => orderChecker.Done(3));

        await _sut.StartupCheckup();

    }

}