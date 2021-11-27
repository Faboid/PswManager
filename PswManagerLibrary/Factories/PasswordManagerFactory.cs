using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Global;
using PswManagerLibrary.Storage;
using PswManagerLibrary.UIConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Factories {
    public class PasswordManagerFactory : IPasswordManagerFactory {

        private ICryptoAccountFactory cryptoAccountFactory;

        public PasswordManagerFactory(ICryptoAccountFactory cryptoAccountFactory) {
            this.cryptoAccountFactory = cryptoAccountFactory;
        }

        public IPasswordManager CreatePasswordManager(IUserInput userInput, IPaths paths, string passPassword, string emaPassword) {

            ICryptoAccount cryptoAccount = cryptoAccountFactory.CreateCryptoAccount(passPassword, emaPassword);

            IToken token = new Token(cryptoAccount, paths, userInput);

            if(token.GetUserConfirmation(out string message) is false) {
                //todo - set up a proper exception for this case
                throw new InvalidCommandException($"The operation has been canceled. Reason: {message}");
            }

            return new PasswordManager(paths, cryptoAccount);

        }
        
    }
}
