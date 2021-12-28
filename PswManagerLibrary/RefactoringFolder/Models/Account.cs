using PswManagerLibrary.RefactoringFolder.Storage;

namespace PswManagerLibrary.RefactoringFolder.Models {

    /// <summary>
    /// A model that stores a single account's information.
    /// </summary>
    public sealed class Account {

        AccountBuilder builder;

        public Account() {

        }

        public Account(AccountBuilder builder, string name = null, string email = null, string password = null) {
            Name = name;
            Email = email;
            Password = password;
            this.builder = builder;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public void Create() {

        }

        public void Read() {

        }

        public void Edit() {

        }

        public void Delete() {

        }

    }
}
