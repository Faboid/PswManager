using PswManagerDatabase.Models;
using System.Collections.Generic;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataReader : IDataHelper {

        ConnectionResult<IEnumerable<AccountModel>> GetAllAccounts();
        ConnectionResult<AccountModel> GetAccount(string name);

    }
}
