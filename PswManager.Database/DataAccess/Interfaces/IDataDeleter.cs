using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces; 
public interface IDataDeleter : IDataHelper {

    Task<Option<DeleterErrorCode>> DeleteAccountAsync(string name);

}
