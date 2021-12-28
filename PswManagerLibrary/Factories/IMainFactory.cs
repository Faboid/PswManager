namespace PswManagerLibrary.Factories {
    public interface IMainFactory {

        public IPasswordManagerFactory GetPasswordManagerFactory();

        public ICryptoAccountFactory GetCryptoAccountFactory();

    }
}
