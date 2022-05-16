using PswManager.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces {
    public interface IDataReader : IDataHelper {

        ConnectionResult<IEnumerable<AccountResult>> GetAllAccounts();
        ConnectionResult<AccountModel> GetAccount(string name);

        Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsAsync();
        ValueTask<ConnectionResult<AccountModel>> GetAccountAsync(string name);

    }
}
