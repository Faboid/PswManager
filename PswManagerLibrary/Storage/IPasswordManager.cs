
namespace PswManagerLibrary.Storage {
    public interface IPasswordManager {

        public void CreatePassword(string name, string password, string email);

        public string GetPassword(string name);

        public void EditPassword(string name, string[] arguments);

        public void DeletePassword(string name);

    }
}
