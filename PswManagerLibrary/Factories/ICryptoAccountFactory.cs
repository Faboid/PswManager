using PswManagerLibrary.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Factories {
    public interface ICryptoAccountFactory {

        public ICryptoAccount CreateCryptoAccount(string passPassword, string emaPassword);

    }
}
