using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces {
    public interface IDataReader : IDataHelper {

        ConnectionResult<IEnumerable<AccountResult>> GetAllAccounts();
        Option<AccountModel, ReaderErrorCode> GetAccount(string name);

        Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsAsync();
        ValueTask<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name);

    }
}
