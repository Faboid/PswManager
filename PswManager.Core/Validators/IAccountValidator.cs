using PswManager.Database.Models;

namespace PswManager.Core.Validators;
public interface IAccountValidator {

    public NameValid IsNameValid(AccountModel account);
    public PasswordValid IsPasswordValid(AccountModel account);
    public EmailValid IsEmailValid(AccountModel account);
    public AccountValid IsAccountValid(AccountModel account);

}

public enum AccountValid {
    Unknown,
    Valid,
    NameEmptyOrNull,
    PasswordEmptyOrNull,
    EmailEmptyOrNull,
}

public enum NameValid {
    Unknown,
    Valid,
    EmptyOrNull
}

public enum PasswordValid {
    Unknown,
    Valid,
    EmptyOrNull
}

public enum EmailValid {
    Unknown,
    Valid,
    EmptyOrNull
}