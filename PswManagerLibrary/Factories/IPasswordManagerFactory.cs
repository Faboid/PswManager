using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Global;
using PswManagerLibrary.Storage;
using PswManagerLibrary.UIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Factories {
    public interface IPasswordManagerFactory {

        IPasswordManager CreatePasswordManager(IUserInput userInput, IPaths paths, string passPassword, string emaPassword);

    }
}
