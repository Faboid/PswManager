using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;

namespace PswManager.Core.Tests.Mocks {
    internal class OptionMocks {

        public static Option<CreatorErrorCode> ValidateValues(AccountModel model) {

            if(string.IsNullOrWhiteSpace(model.Name)) return CreatorErrorCode.InvalidName;
            if(string.IsNullOrWhiteSpace(model.Password)) return CreatorErrorCode.MissingPassword;
            if(string.IsNullOrWhiteSpace(model.Email)) return CreatorErrorCode.MissingEmail;

            return Option.None<CreatorErrorCode>();
        }

    }
}
