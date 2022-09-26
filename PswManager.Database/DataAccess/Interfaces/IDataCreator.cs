using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces; 
public interface IDataCreator : IDataHelper {

    Task<Option<CreatorErrorCode>> CreateAccountAsync(AccountModel model);

}
