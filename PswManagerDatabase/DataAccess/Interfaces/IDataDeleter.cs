using PswManagerDatabase.Models;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataDeleter : IDataHelper {
    
        ConnectionResult DeleteAccount(string name);

    }
}
