using PswManager.Core.AccountModels;

namespace PswManager.Core.Validators;
public interface IAccountValidator {

    public NameValid IsNameValid(IExtendedAccountModel account);
    public PasswordValid IsPasswordValid(IExtendedAccountModel account);
    public EmailValid IsEmailValid(IExtendedAccountModel account);
    public AccountValid IsAccountValid(IExtendedAccountModel account);

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