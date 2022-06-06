using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountReader {

        Option<AccountModel, ReaderErrorCode> ReadAccount(string name);
        Task<Option<AccountModel, ReaderErrorCode>> ReadAccountAsync(string name);

        //todo - consider turning this into IQueryable<T>
        Result<IEnumerable<AccountResult>> ReadAllAccounts();
        Task<Result<IAsyncEnumerable<AccountResult>>> ReadAllAccountsAsync();

    }
}
