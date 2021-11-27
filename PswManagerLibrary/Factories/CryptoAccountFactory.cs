using PswManagerLibrary.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Factories {
    public class CryptoAccountFactory : ICryptoAccountFactory {

        public ICryptoAccount CreateCryptoAccount(string passPassword, string emaPassword) {
            return new CryptoAccount(passPassword, emaPassword);
        }

    }
}
