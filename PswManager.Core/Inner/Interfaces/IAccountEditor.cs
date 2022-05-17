using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountEditor {

        Result<AccountModel> UpdateAccount(string name, AccountModel newValues);
        AsyncResult<AccountModel> UpdateAccountAsync(string name, AccountModel newValues);

    }
}
