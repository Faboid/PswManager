using PswManager.Core.AccountModels;
using PswManager.Core.MasterKey;
using PswManager.Core.Services;
using PswManager.Database;
using PswManager.Database.Models;
using PswManager.Encryption.Services;

namespace PswManager.Core.Tests.MasterKeyTests;

public class AccountsHandlerTests {

    public AccountsHandlerTests() {
        var dataFactory = new DataFactory(DatabaseType.InMemory);
        _dataConnection = dataFactory.GetDataConnection();

        char[] firstPassword = "firstPassword".ToCharArray();
        char[] secondPassword = "secondPassword".ToCharArray();

        var oldPassCrypto = new CryptoService(firstPassword.Take(5).ToArray(), "test.1");
        var oldEmaCrypto = new CryptoService(firstPassword.Skip(5).ToArray(), "test.1");
        var newPassCrypto = new CryptoService(secondPassword.Take(5).ToArray(), "test.2");
        var newEmaCrypto = new CryptoService(secondPassword.Skip(5).ToArray(), "test.2");

        var oldCryptoAccountService = new CryptoAccountService(oldPassCrypto, oldEmaCrypto);
        var newCryptoAccountService = new CryptoAccountService(newPassCrypto, newEmaCrypto);

        _startingFactory = new AccountModelFactory(oldCryptoAccountService);
        _newFactory = new AccountModelFactory(newCryptoAccountService);

        _rawAccounts = _rawAccounts.OrderBy(x => x.Name).ToArray();
        _accounts = _rawAccounts.Select(x => _startingFactory.CreateDecryptedAccount(x)).Select(x => x.GetEncryptedAccount()).OrderBy(x => x.Name).ToArray();
        _extendedAccounts = _accounts.Select(x => _startingFactory.CreateEncryptedAccount(x)).OrderBy(x => x.Name).ToArray();

        _sut = new(_dataConnection, _startingFactory);
    }

    private readonly AccountsHandler _sut;
    private readonly IDataConnection _dataConnection;
    private readonly IAccountModelFactory _startingFactory;
    private readonly IAccountModelFactory _newFactory;

    private readonly IReadOnlyAccountModel[] _rawAccounts = {
        new AccountModel("FirstName", "FirstPassword", "FirstEmail"),
        new AccountModel("Another", "here", "email@hello.com"),
        new AccountModel("And third here", "Sup", "anotheremai"),
    };

    private readonly IReadOnlyAccountModel[] _accounts;
    private readonly IExtendedAccountModel[] _extendedAccounts;

    public async Task Reset() {
        await _dataConnection
            .EnumerateAccountsAsync()
            .Select(x => x.OrDefault())
            .Where(x => x is not null)
            .ForEachAsync(x => _dataConnection.DeleteAccountAsync(x.Name));

        foreach(var account in _accounts) {
            await _dataConnection.CreateAccountAsync(account);
        }
    }

    [Fact]
    public async Task GetsAccountsRetrievesAll() {

        await Reset();
        var accounts = await _sut.GetAccounts();

        Assert.Equal(_accounts.Length, accounts.Length);
        
        for(int i = 0; i < _accounts.Length; i++) {
            AssertEqualAccounts(_accounts[i], accounts[i]);
        }

    }

    [Fact]
    public async Task DeleteAccounts_DeleteAll() {

        await Reset();
        await _sut.DeleteAccounts(_extendedAccounts);
        Assert.False(await _dataConnection.EnumerateAccountsAsync().AnyAsync());

    }

    [Fact]
    public async Task RecreateAccounts() {

        await Reset();
        var accounts = await _dataConnection
            .EnumerateAccountsAsync()
            .Select(x => x.OrDefault())
            .Where(x => x is not null).
            ToListAsync();

        //delete all
        var tasks = accounts.Select(x => _dataConnection.DeleteAccountAsync(x.Name));
        await Task.WhenAll(tasks);

        await _sut.RecreateAccounts(_extendedAccounts);

        var recreatedAccounts = await _dataConnection
            .EnumerateAccountsAsync()
            .Select(x => x.OrDefault()).Where(x => x is not null)
            .OrderBy(x => x.Name)
            .ToArrayAsync();

        Assert.Equal(_extendedAccounts.Length, recreatedAccounts.Length);
        for(int i = 0; i < _accounts.Length; i++) {
            AssertEqualAccounts(_extendedAccounts[i], recreatedAccounts[i]);
        }

    }

    [Fact]
    public async Task RebuildAccounts_AreEncryptedWithNewCrypto() {

        await Reset();
        var accounts = (await _sut.RebuildAccounts(_extendedAccounts, _newFactory)).OrderBy(x => x.Name).ToArray();

        //to test if they're using the new encryption
        Assert.Equal(_extendedAccounts.Length, accounts.Length);
        for(int i = 0; i < _accounts.Length; i++) {
            AssertNotEqualAccounts(_extendedAccounts[i], accounts[i]);

            //create an encrypted model with the new factory and try to decrypt it to check if it's using the new crypto
            var account = accounts[i];
            var model = _newFactory.CreateEncryptedAccount(account);
            var decrypted = model.GetDecryptedAccount();
            AssertEqualAccounts(_rawAccounts[i], decrypted);

        }

    }

    private static void AssertEqualAccounts(IReadOnlyAccountModel expected, IReadOnlyAccountModel actual) {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Password, actual.Password);
        Assert.Equal(expected.Email, actual.Email);
    }

    private static void AssertNotEqualAccounts(IReadOnlyAccountModel expected, IReadOnlyAccountModel actual) {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        Assert.Equal(expected.Name, actual.Name);
        Assert.NotEqual(expected.Password, actual.Password);
        Assert.NotEqual(expected.Email, actual.Email);
    }

}