namespace PswManager.Database.Models {
    public class AccountModel {

        public AccountModel() {

        }

        public AccountModel(string name, string password, string email) {
            Name = name;
            Password = password;
            Email = email;
        }

        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public override string ToString() {
            return $"{Name} {Password} {Email}";
        }

    }
}
