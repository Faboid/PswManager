using PswManager.Core.Inner.Interfaces;
using PswManager.Core.Services;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Core.Inner;
public class AccountReader : IAccountReader {

    private readonly IDataReader dataReader;
    private readonly ICryptoAccountService cryptoAccount;

    public AccountReader(IDataReader dataReader, ICryptoAccountService cryptoAccount) {
        this.dataReader = dataReader;
        this.cryptoAccount = cryptoAccount;
    }

    public Option<AccountModel, ReaderErrorCode> ReadAccount(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return ReaderErrorCode.InvalidName;
        }

        var result = dataReader.GetAccount(name);
        return result.Bind<AccountModel>(x => cryptoAccount.Decrypt(x));
    }

    public async Task<Option<AccountModel, ReaderErrorCode>> ReadAccountAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return ReaderErrorCode.InvalidName;
        }

        var result = await dataReader.GetAccountAsync(name).ConfigureAwait(false);
        return await result.BindAsync(x => Task.Run<Option<AccountModel, ReaderErrorCode>>(() => cryptoAccount.Decrypt(x)));
    }

    public Option<IEnumerable<NamedAccountOption>, ReaderAllErrorCode> ReadAllAccounts() {
        return dataReader
            .GetAllAccounts()
            .Bind<IEnumerable<NamedAccountOption>>(x => new(DecryptAll(x)));
    }

    public async Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> ReadAllAccountsAsync() {
        return (await dataReader.GetAllAccountsAsync())
            .Bind<IAsyncEnumerable<NamedAccountOption>>(x => new(DecryptAllAsync(x)));
    }

    private IEnumerable<NamedAccountOption> DecryptAll(IEnumerable<NamedAccountOption> enumerable) {
        foreach(var option in enumerable) {
            yield return option.Bind<AccountModel>(x => cryptoAccount.Decrypt(x));
        }
    }

    private async IAsyncEnumerable<NamedAccountOption> DecryptAllAsync(IAsyncEnumerable<NamedAccountOption> enumerable) {
        await foreach(var option in enumerable) {
            yield return await option.BindAsync<AccountModel>(async x => await Task.Run(() => cryptoAccount.Decrypt(x)).ConfigureAwait(false));
        }
    }

}
