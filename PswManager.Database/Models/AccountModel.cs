namespace PswManager.Database.Models; 
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

    internal bool IsAllValid(out AccountValid result) {

        if(string.IsNullOrWhiteSpace(Name)) {
            result = AccountValid.MissingName;
            return false;
        }

        if(string.IsNullOrWhiteSpace(Password)) {
            result = AccountValid.MissingPassword;
            return false;
        }

        if(string.IsNullOrWhiteSpace(Email)) {
            result = AccountValid.MissingEmail;
            return false;
        }

        result = AccountValid.Valid;
        return true;
    }

}

internal enum AccountValid {
    Undefined,
    Valid,
    MissingName,
    MissingPassword,
    MissingEmail
}

