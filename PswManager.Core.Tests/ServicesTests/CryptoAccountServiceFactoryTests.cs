using PswManager.Core.Services;
using PswManager.Encryption.Cryptography;
using PswManager.Encryption.Services;
using System.Linq;

namespace PswManager.Core.Tests.ServicesTests;

public class CryptoAccountServiceFactoryTests {

    public CryptoAccountServiceFactoryTests() {
        _sut = new CryptoAccountServiceFactory(_tokenServiceMock.Object, _cryptoServiceFactoryMock.Object, _keyGeneratorServiceFactoryMock.Object);
    }

    private KeyGeneratorService GetKeyGeneratorService(char[] pass) => new(new byte[] { 45, 12, 43 }, pass, 1000);

    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<ICryptoService> _cryptoServiceMock = new();
    private readonly Mock<ICryptoServiceInternalFactory> _cryptoServiceFactoryMock = new();
    private readonly Mock<IKeyGeneratorServiceInternalFactory> _keyGeneratorServiceFactoryMock = new();
    private readonly Mock<IKeyGeneratorService> _keyGeneratorServiceMock = new();

    private readonly ICryptoAccountServiceFactory _sut;

    private void ResetAllMocks() {
        _tokenServiceMock.Reset();
        _cryptoServiceMock.Reset();
        _cryptoServiceFactoryMock.Reset();
        _keyGeneratorServiceMock.Reset();
        _keyGeneratorServiceFactoryMock.Reset();
    }

    [Fact]
    public async Task SignUpCreatesANewToken() {

        char[] password = "hello".ToCharArray();
        Key masterKey = new("MasterKey".ToCharArray());
        ResetAllMocks();
        _cryptoServiceFactoryMock.Setup(x => x.GetCryptoService(masterKey)).Returns(_cryptoServiceMock.Object);
        _keyGeneratorServiceFactoryMock.Setup(x => x.CreateGeneratorService(password)).Returns(_keyGeneratorServiceMock.Object);
        _keyGeneratorServiceMock.Setup(x => x.GenerateKeyAsync()).Returns(Task.FromResult(masterKey));

        _ = await _sut.SignUpAccountAsync(password);

        _tokenServiceMock.Verify(x => x.SetToken(_cryptoServiceMock.Object));

    }

    [Fact]
    public async Task LogInChecksToken() {

        char[] password = "hello".ToCharArray();
        Key masterKey = new("MasterKey".ToCharArray());
        ResetAllMocks();
        _cryptoServiceFactoryMock.Setup(x => x.GetCryptoService(masterKey)).Returns(_cryptoServiceMock.Object);
        _keyGeneratorServiceFactoryMock.Setup(x => x.CreateGeneratorService(password)).Returns(_keyGeneratorServiceMock.Object);
        _keyGeneratorServiceMock.Setup(x => x.GenerateKeyAsync()).Returns(Task.FromResult(masterKey));

        _ = await _sut.LogInAccountAsync(password);

        _tokenServiceMock.Verify(x => x.VerifyToken(_cryptoServiceMock.Object));

    }

    [Fact]
    public async Task BuildService_DoesNotUseToken() {

        char[] password = "hello".ToCharArray();
        Key masterKey = new("MasterKey".ToCharArray());
        ResetAllMocks();
        _keyGeneratorServiceFactoryMock.Setup(x => x.CreateGeneratorService(password)).Returns(_keyGeneratorServiceMock.Object);
        _keyGeneratorServiceMock.Setup(x => x.GenerateKeyAsync()).Returns(Task.FromResult(masterKey));

        _ = await _sut.BuildCryptoAccountService(password);

        _tokenServiceMock.VerifyNoOtherCalls();

    }

    [Fact]
    public async Task KeyGeneratorServiceIsDisposed() {

        char[] password = "hello".ToCharArray();
        Key masterKey = new("MasterKey".ToCharArray());
        ResetAllMocks();
        _keyGeneratorServiceFactoryMock.Setup(x => x.CreateGeneratorService(password)).Returns(_keyGeneratorServiceMock.Object);
        _keyGeneratorServiceMock.Setup(x => x.GenerateKeyAsync()).Returns(Task.FromResult(masterKey));

        var cryptoAccount = await _sut.BuildCryptoAccountService(password);
        _ = cryptoAccount.GetEmaCryptoService();
        _ = cryptoAccount.GetPassCryptoService();

        _keyGeneratorServiceMock.Verify(x => x.DisposeAsync());
        _keyGeneratorServiceMock.Invocations.Clear();

        cryptoAccount = await _sut.BuildCryptoAccountService(password);
        _ = cryptoAccount.GetEmaCryptoService();
        _ = cryptoAccount.GetPassCryptoService();

        _keyGeneratorServiceMock.Verify(x => x.DisposeAsync());
        _keyGeneratorServiceMock.Invocations.Clear();

        cryptoAccount = await _sut.BuildCryptoAccountService(password);
        _ = cryptoAccount.GetEmaCryptoService();
        _ = cryptoAccount.GetPassCryptoService();

        _keyGeneratorServiceMock.Verify(x => x.DisposeAsync());
        _keyGeneratorServiceMock.Invocations.Clear();

    }

    [Fact]
    public async Task GeneratedTokenKeysAreConsistent() {

        static char[] GetPassword() => "hello".ToCharArray();

        ResetAllMocks();
        _keyGeneratorServiceFactoryMock.Setup(x => x.CreateGeneratorService(It.IsAny<char[]>())).Returns<char[]>(x => GetKeyGeneratorService(GetPassword()));

        var generator = _keyGeneratorServiceFactoryMock.Object.CreateGeneratorService(GetPassword());
        var expectedTokenKey = await generator.GenerateKeyAsync();

        _ = await _sut.LogInAccountAsync(GetPassword());
        _cryptoServiceFactoryMock.Verify(x => x.GetCryptoService(It.Is<Key>(x => x == expectedTokenKey)));
        _cryptoServiceFactoryMock.Invocations.Clear();

        _ = await _sut.SignUpAccountAsync(GetPassword());
        _cryptoServiceFactoryMock.Verify(x => x.GetCryptoService(expectedTokenKey));
        _cryptoServiceFactoryMock.Invocations.Clear();

    }

    [Fact]
    public async Task GeneratedCryptoAccountServicesAreConsistent() {

        static char[] GetPassword() => "hello".ToCharArray();
        ResetAllMocks();
        _keyGeneratorServiceFactoryMock.Setup(x => x.CreateGeneratorService(It.IsAny<char[]>())).Returns<char[]>(x => GetKeyGeneratorService(GetPassword()));
        _tokenServiceMock.Setup(x => x.VerifyToken(It.IsAny<ICryptoService>())).Returns(ITokenService.TokenResult.Success);

        var loginCrypto = (await _sut.LogInAccountAsync(GetPassword())).OrDefault() ?? throw new ArgumentNullException();
        var signupCrypto = await _sut.SignUpAccountAsync(GetPassword());
        var buildCrypto = await _sut.BuildCryptoAccountService(GetPassword());

        var loginPassCrypto = loginCrypto.GetPassCryptoService();
        var loginEmaCrypto = loginCrypto.GetEmaCryptoService();
        var signupPassCrypto = signupCrypto.GetPassCryptoService();
        var signupEmaCrypto = signupCrypto.GetEmaCryptoService();
        var buildPassCrypto = buildCrypto.GetPassCryptoService();
        var buildEmaCrypto = buildCrypto.GetEmaCryptoService();

        AssertEqual(loginEmaCrypto, signupEmaCrypto, buildEmaCrypto);
        AssertEqual(loginPassCrypto, signupPassCrypto, buildPassCrypto);

    }

    private static void AssertEqual(ICryptoService loginCrypto, ICryptoService signupCrypto, ICryptoService buildCrypto) {
        Assert.Equal(loginCrypto, signupCrypto);
        Assert.Equal(buildCrypto, loginCrypto);
        Assert.Equal(signupCrypto, buildCrypto);
    }

}