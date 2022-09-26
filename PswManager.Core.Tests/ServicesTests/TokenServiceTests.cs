using PswManager.Encryption.Cryptography;
using PswManager.Utils;
using PswManager.Encryption.Services;
using PswManager.Core.Services;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace PswManager.Core.Tests.ServicesTests;
public class TokenServiceTests {

    public TokenServiceTests() {
        var fileSystem = new MockFileSystem();
        _fileInfoFactory = new MockFileInfoFactory(fileSystem);
        fileSystem.Directory.CreateDirectory(PathsBuilder.GetDataDirectory);
        _fileInfo = _fileInfoFactory.FromFileName(GetPath);
        _sut = new TokenService(_fileInfo, _token);
    }

    static ICryptoService CreateCryptoService(Key password) => new CryptoService(password, "test.1");
    static Key GetPassword => new("password".ToCharArray());
    static string GetPath => Path.Combine(PathsBuilder.GetDataDirectory, "GenericTokenTestsPath.txt");

    private readonly string _token = "SomeToken";
    private readonly IFileInfoFactory _fileInfoFactory;
    private readonly IFileInfo _fileInfo;
    private readonly ICryptoService _cryptoService = CreateCryptoService(GetPassword);
    private readonly ITokenService _sut;

    [Fact]
    public void IsSet_FalseBefore_TrueAfter() {

        var before = _sut.IsSet();
        _sut.SetToken(_cryptoService);
        var after = _sut.IsSet();

        Assert.False(before);
        Assert.True(after);

    }

    [Fact]
    public void SetsTokenAndVerify() {

        var beforeSettingResult = _sut.VerifyToken(_cryptoService);
        var newCrypto = CreateCryptoService(new("SomePassword".ToCharArray()));
        _sut.SetToken(newCrypto);
        var wrongResult = _sut.VerifyToken(_cryptoService);
        var correctResult = _sut.VerifyToken(newCrypto);

        Assert.Equal(ITokenService.TokenResult.Missing, beforeSettingResult);
        Assert.Equal(ITokenService.TokenResult.Failure, wrongResult);
        Assert.Equal(ITokenService.TokenResult.Success, correctResult);

    }

}
