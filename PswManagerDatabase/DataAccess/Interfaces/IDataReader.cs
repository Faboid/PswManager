using PswManagerDatabase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataReader : IDataHelper {

        ConnectionResult<IEnumerable<AccountResult>> GetAllAccounts();
        ConnectionResult<AccountModel> GetAccount(string name);

        Task<ConnectionResult<IAsyncEnumerable<AccountResult>>> GetAllAccountsAsync();

    }
}
