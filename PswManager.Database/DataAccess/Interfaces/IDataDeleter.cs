using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces {
    public interface IDataDeleter : IDataHelper {
    
        Option<DeleterErrorCode> DeleteAccount(string name);
        ValueTask<Option<DeleterErrorCode>> DeleteAccountAsync(string name);

    }
}
