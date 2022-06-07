using PswManager.Database.Models;
using PswManager.Utils;

namespace PswManager.Core.Inner {
    internal static class AccountExtensions {

        static readonly Result nullOrWhiteSpaceNameResult = new("You must provide a valid account name.");
        static readonly Result nullOrWhiteSpacePasswordResult = new("You must provide a valid password.");
        static readonly Result nullOrWhiteSpaceEmailResult = new("You must provide a valid email.");

        public static bool IsAnyValueNullOrEmpty(this AccountModel account, out Result failureResult) {

            if(string.IsNullOrWhiteSpace(account.Name)) {
                failureResult = nullOrWhiteSpaceNameResult;
                return true;
            }

            if(string.IsNullOrWhiteSpace(account.Password)) {
                failureResult = nullOrWhiteSpacePasswordResult;
                return true;
            }

            if(string.IsNullOrWhiteSpace(account.Email)) {
                failureResult = nullOrWhiteSpaceEmailResult;
                return true;
            }

            failureResult = null;
            return false;

        }

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
