using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;

namespace PswManager.Core.Inner.Interfaces {
    interface IAccountCreator {

        Result CreateAccount(AccountModel model);
        AsyncResult CreateAccountAsync(AccountModel model);

    }
}
