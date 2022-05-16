using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces {
    public interface IDataDeleter : IDataHelper {
    
        ConnectionResult DeleteAccount(string name);

        ValueTask<ConnectionResult> DeleteAccountAsync(string name);

    }
}
