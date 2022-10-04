using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.Database.Interfaces;
public interface IDataHelper {

    //todo - consider whether to leave it as enum-only or to use Option<bool, AccountExistsErrorCode> with a less comprehensive code set
    AccountExistsStatus AccountExist(string name);
    Task<AccountExistsStatus> AccountExistAsync(string name);

}
