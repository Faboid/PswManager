
namespace PswManagerLibrary {
    public interface IPasswordManager {

        public void CreatePassword(AccountModel account);

        public AccountModel GetPassword(string name);

        public void EditPassword(AccountModel account, string newPassword);

        public void DeletePassword(AccountModel account);

    }
}
