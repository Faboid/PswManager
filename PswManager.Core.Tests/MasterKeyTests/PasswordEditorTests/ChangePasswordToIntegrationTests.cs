using PswManager.Core.AccountModels;
using PswManager.Core.IO;
using PswManager.Core.MasterKey;
using PswManager.Core.Services;
using PswManager.Database;
using PswManager.Database.Models;
using PswManager.Encryption.Services;
using PswManager.Paths;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using static PswManager.Core.Services.ITokenService;

namespace PswManager.Core.Tests.MasterKeyTests.PasswordEditorTests;

public class ChangePasswordToIntegrationTests {

	public ChangePasswordToIntegrationTests() {
		_fileSystem = new();
		_fileInfoFactory = new MockFileInfoFactory(_fileSystem);
		_directoryInfoFactory = new MockDirectoryInfoFactory(_fileSystem);
		_directoryInfoWrapperFactory = new DirectoryInfoWrapperFactory(_directoryInfoFactory);
		_pathsBuilder = new PathsBuilder(_directoryInfoFactory);

		_dataConnection = new DataFactory(DatabaseType.InMemory).GetDataConnection();

		_tokenFile = _fileInfoFactory.FromFileName(_pathsBuilder.GetTokenPath());
        _tokenService = new TokenService(_tokenFile, "someToken");
		_cryptoServiceFactory = new MockCryptoServiceFactory();
		_cryptoAccountServiceFactory = new CryptoAccountServiceFactory(_tokenService, _cryptoServiceFactory, new MockKeyGeneratorServiceFactory("hello".Select(x => (byte)x).ToArray(), 1000));
	}

	private readonly MockFileSystem _fileSystem;
	private readonly IFileInfoFactory _fileInfoFactory;
	private readonly IDirectoryInfoFactory _directoryInfoFactory;
	private readonly IDirectoryInfoWrapperFactory _directoryInfoWrapperFactory;

	private readonly IFileInfo _tokenFile;
	private readonly ITokenService _tokenService;
	private readonly ICryptoServiceFactory _cryptoServiceFactory;
	private readonly ICryptoAccountServiceFactory _cryptoAccountServiceFactory;
	private readonly IDataConnection _dataConnection;
	private readonly IPathsBuilder _pathsBuilder;

	private async Task<PasswordEditor> GetEditor(string password) {
		var cryptoAccService = await _cryptoAccountServiceFactory.SignUpAccountAsync(password.ToCharArray());
		var accModel = new AccountModelFactory(cryptoAccService);
        return new(_directoryInfoWrapperFactory, _fileInfoFactory, _pathsBuilder, _dataConnection, accModel, _cryptoAccountServiceFactory);
    }

	private async Task Reset() {
		await _dataConnection.EnumerateAccountsAsync().ForEachAsync(x => _dataConnection.DeleteAccountAsync(x.OrDefault().Name));
	}

	[Fact]
	public async Task ChangePassword_TokenIsUpdated() {

		await Reset();
		string startingPassword = "Starting";
		string newPassword = "NewPass";
		var sut = await GetEditor(startingPassword);

		var startingResult = _tokenService.VerifyToken(await _cryptoAccountServiceFactory.GetTokenCrypto(startingPassword.ToCharArray()));
		Assert.Equal(TokenResult.Success, startingResult);

		await sut.ChangePasswordTo(newPassword);
        var result = _tokenService.VerifyToken(await _cryptoAccountServiceFactory.GetTokenCrypto(newPassword.ToCharArray()));
        Assert.Equal(TokenResult.Success, result);

    }

	[Fact]
	public async Task FailPasswordChange_TokenStaysUnchanged() {

		await Reset();
		string password = "SomePass";
		var connMock = new Mock<IDataConnection>();
		var cryptoAccService = await _cryptoAccountServiceFactory.SignUpAccountAsync(password.ToCharArray());
        var accModel = new AccountModelFactory(cryptoAccService);
        var sut = new PasswordEditor(_directoryInfoWrapperFactory, _fileInfoFactory, _pathsBuilder, connMock.Object, accModel, _cryptoAccountServiceFactory);

		var model = cryptoAccService.Encrypt(new AccountModel("Name", "Password", "Email"));
		await _dataConnection.CreateAccountAsync(model);
		connMock.Setup(x => x.EnumerateAccountsAsync()).Returns(() => _dataConnection.EnumerateAccountsAsync());
		connMock.Setup(x => x.UpdateAccountAsync(It.IsAny<string>(), It.IsAny<IReadOnlyAccountModel>())).Throws<InvalidOperationException>();

        var before = _tokenService.VerifyToken(await _cryptoAccountServiceFactory.GetTokenCrypto(password.ToCharArray()));
        Assert.Equal(TokenResult.Success, before);

        await sut.ChangePasswordTo("Someother");

        var after = _tokenService.VerifyToken(await _cryptoAccountServiceFactory.GetTokenCrypto(password.ToCharArray()));
        Assert.Equal(TokenResult.Success, after);

    }

	[Fact]
	public async Task ChangePasswords_AccountsAreEncryptedWithNewPassword() {

		await Reset();
		string password = "StartingPassword";
		string newPassword = "NewPassword";
        var cryptoAccService = await _cryptoAccountServiceFactory.SignUpAccountAsync(password.ToCharArray());
		var sut = await GetEditor(password);

        var model = new AccountModel("Name", "Password", "Email");
		await _dataConnection.CreateAccountAsync(cryptoAccService.Encrypt(model));

		await sut.ChangePasswordTo(newPassword);

		var newCryptoAccService = await _cryptoAccountServiceFactory.BuildCryptoAccountService(newPassword.ToCharArray());
		var accountOption = await _dataConnection.EnumerateAccountsAsync().FirstAsync();
		var decryptedAcc = newCryptoAccService.Decrypt(accountOption.OrDefault());

		Assert.Equal(model.Name, decryptedAcc.Name);
		Assert.Equal(model.Password, decryptedAcc.Password);
		Assert.Equal(model.Email, decryptedAcc.Email);

    }

}