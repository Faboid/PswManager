using PswManagerDatabase.Models;
using System.Collections.Generic;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataReader : IDataHelper {

        ConnectionResult<List<AccountModel>> GetAllAccounts();
        ConnectionResult<AccountModel> GetAccount(string name);

    }
}
