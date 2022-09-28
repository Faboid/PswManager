using PswManager.Core.Services;
using PswManager.Database.Models;

namespace PswManager.Core.Tests.Mocks;
public static class ConnectionResultMocks {

    public static ConnectionResult<AccountModel> SuccessIfAllValuesAreNotEmpty(AccountModel model) {
        bool[] isAnyNullOrEmpty = new bool[] {
            string.IsNullOrWhiteSpace(model.Name),
            string.IsNullOrWhiteSpace(model.Password),
            string.IsNullOrWhiteSpace(model.Email)
        };

        return new(!isAnyNullOrEmpty.Any(x => x), model);
    }

    public static ConnectionResult<IEnumerable<AccountResult>> GenerateInfiniteEncryptedAccountList(ICryptoAccountService cryptoAccount) {
        return new(true, AccountModelMocks.GenerateManyEncrypted(cryptoAccount).Select(x => new AccountResult(x.Name, x)));
    }

    public static ConnectionResult<IAsyncEnumerable<AccountResult>> GenerateInfiniteEncryptedAccountListAsync(ICryptoAccountService cryptoAccount) {
        return new(true, AccountModelMocks.GenerateManyEncryptedAsync(cryptoAccount).Select(x => new AccountResult(x.Name, x)));
    }

}
