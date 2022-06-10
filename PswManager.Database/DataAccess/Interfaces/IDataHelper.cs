using PswManager.Utils;
using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces {
    public interface IDataHelper {

        //todo - consider whether to leave it as enum-only or to use Option<bool, AccountExistsErrorCode> with a less comprehensive code set
        AccountExistsStatus AccountExist(string name);
        ValueTask<AccountExistsStatus> AccountExistAsync(string name);

    }
}
