using PswManager.Core.IO;
using PswManager.Core.MasterKey;
using PswManager.Paths;
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
        _bufferDirectoryMock.Setup(x => x.Exists).Returns(true);
        await _sut.Restore();
        _dataDirectoryMock.Verify(x => x.Delete(true));
        _bufferDirectoryMock.Verify(x => x.CopyToAsync(_dataDirectoryMock.Object.FullName));

    }

    [Fact]
    public async Task RestoreFails_ThrowDirectoryNotFoundException() {

        ResetMocks();
        _bufferDirectoryMock.Setup(x => x.Exists).Returns(false);
        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => _sut.Restore());

    }

    [Fact]
    public async Task EnsureLogsArePreserved() {

        var orderList = new List<int>();
        ResetMocks();
        _bufferDirectoryMock.Setup(x => x.Exists).Returns(true);

        _logsDirectoryMock.Setup(x => x.CopyToAsync(Path.Combine(_bufferDirectoryMock.Object.FullName, "Logs"))).Callback(() => orderList.Add(0));
        _dataDirectoryMock.Setup(x => x.Delete(true)).Callback(() => orderList.Add(1));
        _bufferDirectoryMock.Setup(x => x.CopyToAsync(_dataDirectoryMock.Object.FullName)).Callback(() => orderList.Add(2));
        
        await _sut.Restore();

        Assert.Equal(0, orderList[0]);
        Assert.Equal(1, orderList[1]);
        Assert.Equal(2, orderList[2]);

    }

}