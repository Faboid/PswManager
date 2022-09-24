using PswManager.Database.Models;

namespace PswManager.Core.Validators;

public class AccountValidator : IAccountValidator {
    public AccountValid IsAccountValid(AccountModel account) {
        
        var nameResult = IsNameValid(account);
        if(nameResult != NameValid.Valid) {
            return nameResult switch {
                NameValid.EmptyOrNull => AccountValid.NameEmptyOrNull,
                _ => AccountValid.Unknown,
            };
        }

        var passwordResult = IsPasswordValid(account);
        if(passwordResult != PasswordValid.Valid) {
            return passwordResult switch {
                PasswordValid.EmptyOrNull => AccountValid.PasswordEmptyOrNull,
                _ => AccountValid.Unknown,
            };
        }

        var emailResult = IsEmailValid(account);
        if(emailResult != EmailValid.Valid) {
            return emailResult switch {
                EmailValid.EmptyOrNull => AccountValid.EmailEmptyOrNull,
                _ => AccountValid.Unknown,
            };
        }

        return AccountValid.Valid;
    }

    public EmailValid IsEmailValid(AccountModel account) {
        
        if(string.IsNullOrWhiteSpace(account.Email)) {
            return EmailValid.EmptyOrNull;
        }

        return EmailValid.Valid;
    }

    public NameValid IsNameValid(AccountModel account) {
        
        if(string.IsNullOrWhiteSpace(account.Name)) {
            return NameValid.EmptyOrNull;
        }

        return NameValid.Valid;
    }

    public PasswordValid IsPasswordValid(AccountModel account) {
        
        if(string.IsNullOrWhiteSpace(account.Password)) {
            return PasswordValid.EmptyOrNull;
        }

        return PasswordValid.Valid;
    }
}