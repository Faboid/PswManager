using PswManagerDatabase.Models;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataEditor : IDataHelper {

        ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel);

    }
}
