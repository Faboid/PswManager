using PswManager.Utils.WrappingObjects;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountDeleter {

        Result DeleteAccount(string name);
        AsyncResult DeleteAccountAsync(string name);

    }
}
