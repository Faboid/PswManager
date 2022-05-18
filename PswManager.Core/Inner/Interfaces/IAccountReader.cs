using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountReader {

        Result<AccountModel> ReadAccount(string name);
        Task<Result<AccountModel>> ReadAccountAsync(string name);

        Result<IEnumerable<AccountModel>> ReadAllAccounts();
        Task<Result<IAsyncEnumerable<AccountModel>>> ReadAllAccountsAsync();

    }
}
