using PswManager.Database.Models;

namespace PswManager.ConsoleUI.Inner;
internal static class AccountExtensions {

    public static ValidationResult IsAnyValueNullOrEmpty(this IReadOnlyAccountModel account) {

        if(string.IsNullOrWhiteSpace(account.Name)) {
            return ValidationResult.MissingName;
        }

        if(string.IsNullOrWhiteSpace(account.Password)) {
            return ValidationResult.MissingPassword;
        }

        if(string.IsNullOrWhiteSpace(account.Email)) {
            return ValidationResult.MissingEmail;
        }

        return ValidationResult.Success;
    }

}

public enum ValidationResult {
    Undefined,
    Success,
    MissingName,
    MissingPassword,
    MissingEmail
}

