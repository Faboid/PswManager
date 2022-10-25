using PswManager.Core.IO;
using PswManager.Core.MasterKey;

namespace PswManager.Core.Tests.MasterKeyTests;

public class BufferHandlerTests {

    public BufferHandlerTests() {
        _sut = new(_bufferDirectoryMock.Object, _dataDirectoryMock.Object);
    }

    private readonly Mock<IDirectoryInfoWrapper> _bufferDirectoryMock = new();
    private readonly Mock<IDirectoryInfoWrapper> _dataDirectoryMock = new();

    private readonly BufferHandler _sut;

    private void ResetMocks() {

        _bufferDirectoryMock.Reset();
        _dataDirectoryMock.Reset();
        _bufferDirectoryMock.Setup(x => x.FullName).Returns(@"C:\Buffer\");
        _dataDirectoryMock.Setup(x => x.FullName).Returns(@"C:\Data\");
        _bufferDirectoryMock.Setup(x => x.CopyToAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _dataDirectoryMock.Setup(x => x.CopyToAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
    
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

}