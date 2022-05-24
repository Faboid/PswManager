using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountReader {

        Result<AccountModel> ReadAccount(string name);
        Task<Result<AccountModel>> ReadAccountAsync(string name);

        //todo - consider turning this into IQueryable<T>
        Result<IEnumerable<Result<AccountModel>>> ReadAllAccounts();
        Task<Result<IAsyncEnumerable<Result<AccountModel>>>> ReadAllAccountsAsync();

    }
}
