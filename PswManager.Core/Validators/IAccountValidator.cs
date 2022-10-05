using PswManager.Core.AccountModels;

namespace PswManager.Core.Validators;

/// <summary>
/// Provides methods to validate accounts.
/// </summary>
public interface IAccountValidator {

    /// <summary>
    /// </summary>
    /// <param name="account"></param>
    /// <returns>Whether the account's name is valid.</returns>
    public NameValid IsNameValid(IExtendedAccountModel account);
    
    /// <summary>
    /// </summary>
    /// <param name="account"></param>
    /// <returns>Whether the account's password is valid.</returns>
    public PasswordValid IsPasswordValid(IExtendedAccountModel account);
    
    /// <summary>
    /// </summary>
    /// <param name="account"></param>
    /// <returns>Whether the account's email is valid.</returns>
    public EmailValid IsEmailValid(IExtendedAccountModel account);
    
    /// <summary>
    /// </summary>
    /// <param name="account"></param>
    /// <returns>Whether the account is valid.</returns>
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