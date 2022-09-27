using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces; 
public interface IDataDeleter : IDataHelper {

    Task<DeleterResponseCode> DeleteAccountAsync(string name);

}
