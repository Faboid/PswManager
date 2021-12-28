using PswManagerLibrary.Global;
using PswManagerLibrary.Storage;
using PswManagerLibrary.UIConnection;

namespace PswManagerLibrary.Factories {
    public interface IPasswordManagerFactory {

        IPasswordManager CreatePasswordManager(IUserInput userInput, IPaths paths, string passPassword, string emaPassword);

    }
}
