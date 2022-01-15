using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataReader : IDataHelper {

        ConnectionResult<List<AccountModel>> GetAllAccounts();
        ConnectionResult<AccountModel> GetAccount(string name);

    }
}
