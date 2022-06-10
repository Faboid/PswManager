using PswManager.Database.Models;

namespace PswManager.Core.Inner {
    internal static class AccountExtensions {

        public static ValidationResult IsAnyValueNullOrEmpty(this AccountModel account) {

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

}
