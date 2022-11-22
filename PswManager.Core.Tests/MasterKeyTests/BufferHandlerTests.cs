using PswManager.Core.IO;
using PswManager.Core.MasterKey;
using PswManager.Paths;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace PswManager.Core.Tests.MasterKeyTests;

public class BufferHandlerTests {

    public BufferHandlerTests() {

        var factory = new Mock<IDirectoryInfoWrapperFactory>();
        factory.Setup(x => x.FromDirectoryName(_paths.GetBufferDataDirectory())).Returns(_bufferDirectoryMock.Object);
        factory.Setup(x => x.FromDirectoryName(_paths.GetDataDirectory())).Returns(_dataDirectoryMock.Object);
        factory.Setup(x => x.FromDirectoryName(_paths.GetLogsDirectory())).Returns(_logsDirectoryMock.Object);

        _sut = new(factory.Object, _paths);
    }

    private readonly Mock<IDirectory> _directoryMock = new();
    private readonly Mock<IFileSystem> _fileSystemMock = new();
    private readonly Mock<IDirectoryInfoWrapper> _bufferDirectoryMock = new();
    private readonly Mock<IDirectoryInfoWrapper> _dataDirectoryMock = new();
    private readonly Mock<IDirectoryInfoWrapper> _logsDirectoryMock = new();
    private readonly PathsBuilder _paths = new(new MockFileSystem().DirectoryInfo);

    private readonly BufferHandler _sut;

    private void ResetMocks() {

        _bufferDirectoryMock.Reset();
        _dataDirectoryMock.Reset();
        _bufferDirectoryMock.Setup(x => x.FullName).Returns(@"C:\Buffer\");
        _dataDirectoryMock.Setup(x => x.FullName).Returns(@"C:\Data\");
        _logsDirectoryMock.Setup(x => x.FullName).Returns(@"C:\Data\Logs");
        _bufferDirectoryMock.Setup(x => x.CopyToAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _dataDirectoryMock.Setup(x => x.CopyToAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _logsDirectoryMock.Setup(x => x.CopyToAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        _bufferDirectoryMock.Setup(x => x.FileSystem).Returns(_fileSystemMock.Object);
        _dataDirectoryMock.Setup(x => x.FileSystem).Returns(_fileSystemMock.Object);
        _fileSystemMock.Setup(x => x.Directory).Returns(_directoryMock.Object);

    }

    [Fact]
    public async Task CreateBufferCorrectly() {

        ResetMocks();
        await _sut.Backup();
        _dataDirectoryMock.Verify(x => x.CopyToAsync(_bufferDirectoryMock.Object.FullName));

    }

    [Fact]
    public void FreesResources_DeletesDirectory() {

        ResetMocks();
        _sut.Free();
        _bufferDirectoryMock.Verify(x => x.Delete(true));

    }

    [Fact]
    public async Task RestoresCorrectly() {

        ResetMocks();
        _directoryMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
        await _sut.Restore();
        _dataDirectoryMock.Verify(x => x.Delete(true));
        _bufferDirectoryMock.Verify(x => x.CopyToAsync(_dataDirectoryMock.Object.FullName));

    }

    [Fact]
    public async Task RestoreFails_ThrowDirectoryNotFoundException() {

        ResetMocks();
        _directoryMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => _sut.Restore());

    }

}