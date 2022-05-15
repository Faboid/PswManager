using PswManagerDatabase.Models;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataDeleter : IDataHelper {
    
        ConnectionResult DeleteAccount(string name);

        ValueTask<ConnectionResult> DeleteAccountAsync(string name);

    }
}
