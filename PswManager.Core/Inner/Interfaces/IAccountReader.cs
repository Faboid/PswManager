using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;
using System.Collections.Generic;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountReader {

        Result<AccountModel> ReadAccount(string name);
        AsyncResult<AccountModel> ReadAccountAsync(string name);

        Result<IEnumerable<AccountModel>> ReadAllAccounts();
        AsyncResult<IAsyncEnumerable<AccountModel>> ReadAllAccountsAsync();

    }
}
