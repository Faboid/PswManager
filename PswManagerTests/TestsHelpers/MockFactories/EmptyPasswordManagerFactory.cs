using PswManagerLibrary.Factories;
using PswManagerLibrary.Global;
using PswManagerLibrary.Storage;
using PswManagerLibrary.UIConnection;
using PswManagerTests.TestsHelpers.MockPasswordManager;

namespace PswManagerTests.TestsHelpers.MockFactories {
    public class EmptyPasswordManagerFactory : IPasswordManagerFactory {
        public IPasswordManager CreatePasswordManager(IUserInput userInput, IPaths paths, string passPassword, string emaPassword) {
            return new EmptyPasswordManager();
        }
    }
}
