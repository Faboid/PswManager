using PswManager.Core.Services;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using PswManager.Database.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PswManager.UI.Console.Inner.Interfaces;

namespace PswManager.UI.Console.Inner;
public class AccountReader : IAccountReader {

    private readonly IDataReader dataReader;
    private readonly ICryptoAccountService cryptoAccount;

    public AccountReader(IDataReader dataReader, ICryptoAccountService cryptoAccount) {
        this.dataReader = dataReader;
        this.cryptoAccount = cryptoAccount;
    }

    public Option<IAccountModel, ReaderErrorCode> ReadAccount(string name) {
        return ReadAccountAsync(name).GetAwaiter().GetResult();
    }

    public async Task<Option<IAccountModel, ReaderErrorCode>> ReadAccountAsync(string name) {
        if(string.IsNullOrWhiteSpace(name)) {
            return ReaderErrorCode.InvalidName;
        }

        var result = await dataReader.GetAccountAsync(name).ConfigureAwait(false);
        return await result.BindAsync(x => Task.Run<Option<IAccountModel, ReaderErrorCode>>(() => new(cryptoAccount.Decrypt(x))));
    }

    public IEnumerable<NamedAccountOption> ReadAllAccounts() {
        return ReadAllAccountsAsync().ToEnumerable();
    }

    public IAsyncEnumerable<NamedAccountOption> ReadAllAccountsAsync() {
        return DecryptAllAsync(dataReader.EnumerateAccountsAsync());
    }

    private async IAsyncEnumerable<NamedAccountOption> DecryptAllAsync(IAsyncEnumerable<NamedAccountOption> enumerable) {
        await foreach(var option in enumerable) {
            yield return await option.BindAsync<IAccountModel>(async x => new(await Task.Run(() => cryptoAccount.Decrypt(x)).ConfigureAwait(false)));
        }
    }

}
