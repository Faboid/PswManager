using PswManager.Core.AccountModels;

namespace PswManager.Core.Validators;
public interface IAccountValidator {

    public NameValid IsNameValid(IAccountModel account);
    public PasswordValid IsPasswordValid(IAccountModel account);
    public EmailValid IsEmailValid(IAccountModel account);
    public AccountValid IsAccountValid(IAccountModel account);

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