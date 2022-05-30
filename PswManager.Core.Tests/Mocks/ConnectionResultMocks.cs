using PswManager.Core.Cryptography;
using PswManager.Database.Models;

namespace PswManager.Core.Tests.Mocks {
    internal static class ConnectionResultMocks {

        public static ConnectionResult<AccountModel> SuccessIfAllValuesAreNotEmpty(AccountModel model) {
            bool[] isAnyNullOrEmpty = new bool[] {
                string.IsNullOrWhiteSpace(model.Name),
                string.IsNullOrWhiteSpace(model.Password),
                string.IsNullOrWhiteSpace(model.Email)
            };

            return new(!isAnyNullOrEmpty.Any(x => x), model);
        }

        public static ConnectionResult<AccountModel> GenerateEncryptedSuccessFromName(string name, ICryptoAccount cryptoAccount) {
            return new(true, AccountModelMocks.GenerateEncryptedFromName(name, cryptoAccount));
        }

    }
}
