namespace PswManager.Database.Models.Extensions;

internal static class AccountModelExtensions {

    public static bool IsValid(this IReadOnlyAccountModel model, out AccountValid result) {
        if(model == null) {
            result = AccountValid.IsNull;
            return false;
        }

        if(string.IsNullOrWhiteSpace(model.Name)) {
            result = AccountValid.MissingName;
            return false;
        }

        if(string.IsNullOrWhiteSpace(model.Password)) {
            result = AccountValid.MissingPassword;
            return false;
        }

        if(string.IsNullOrWhiteSpace(model.Email)) {
            result = AccountValid.MissingEmail;
            return false;
        }

        result = AccountValid.Valid;
        return true;

    }

}