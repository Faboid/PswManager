using PswManagerDatabase.Models;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataCreator : IDataHelper {

        ConnectionResult CreateAccount(AccountModel model);


    }
}
