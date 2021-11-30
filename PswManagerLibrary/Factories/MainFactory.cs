using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Factories {
    //todo - decide what to do with this
    public class MainFactory : IMainFactory {

        readonly ICryptoAccountFactory cryptoAccountFactory;
        readonly IPasswordManagerFactory passwordManagerFactory;

        public MainFactory(ICryptoAccountFactory cryptoAccountFactory, IPasswordManagerFactory passwordManagerFactory) {
            this.cryptoAccountFactory = cryptoAccountFactory;
            this.passwordManagerFactory = passwordManagerFactory;
        }

        public ICryptoAccountFactory GetCryptoAccountFactory() => cryptoAccountFactory;

        public IPasswordManagerFactory GetPasswordManagerFactory() => passwordManagerFactory;
    }
}
