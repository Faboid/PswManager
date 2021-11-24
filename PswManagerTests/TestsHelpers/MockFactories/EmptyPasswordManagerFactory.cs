using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Factories;
using PswManagerLibrary.Global;
using PswManagerLibrary.Storage;
using PswManagerLibrary.UIConnection;
using PswManagerTests.TestsHelpers.MockPasswordManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerTests.TestsHelpers.MockFactories {
    public class EmptyPasswordManagerFactory : IPasswordManagerFactory {
        public IPasswordManager CreatePasswordManager(IUserInput userInput, IPaths paths, CryptoString passCryptoString, CryptoString emaCryptoString) {
            return new EmptyPasswordManager();
        }
    }
}
