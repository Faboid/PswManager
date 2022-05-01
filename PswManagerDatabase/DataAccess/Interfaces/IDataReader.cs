using PswManagerDatabase.Models;
using System.Collections.Generic;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataReader : IDataHelper {

        ConnectionResult<IEnumerable<AccountResult>> GetAllAccounts();
        ConnectionResult<AccountModel> GetAccount(string name);

    }
}
