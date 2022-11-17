using PswManager.Core.MasterKey;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using static PswManager.Core.MasterKey.PasswordStatusChecker;

namespace PswManager.Core.Tests.MasterKeyTests;

public class PasswordStatusCheckerTests {

    public PasswordStatusCheckerTests() {
        var fileSystem = new MockFileSystem();
        _fileInfoFactory = new MockFileInfoFactory(fileSystem);
    }

    private readonly IFileInfoFactory _fileInfoFactory;

    [Theory]
    [InlineData(PasswordStatus.Starting)]
    [InlineData(PasswordStatus.Pending)]
    [InlineData(PasswordStatus.Failed)]
    internal async Task SetAndGet(PasswordStatus expected) {

        var sut = new PasswordStatusChecker(_fileInfoFactory.FromFileName(@"C:\Somename.txt"));
        await sut.SetStatusTo(expected);
        var actual = await sut.GetStatus();
        Assert.Equal(expected, actual);

    }

    [Fact]
    internal async Task SetThenFree() {

        var sut = new PasswordStatusChecker(_fileInfoFactory.FromFileName(@"C:\Some.txt"));
        await sut.SetStatusTo(PasswordStatus.Starting);
        sut.Free();
        var actual = await sut.GetStatus();
        Assert.Equal(PasswordStatus.None, actual);

    }

}