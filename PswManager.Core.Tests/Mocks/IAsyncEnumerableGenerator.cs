using PswManager.Core.Services;
using PswManager.Database.Models;

namespace PswManager.Core.Tests.Mocks;

public class IAsyncEnumerableGenerator {

    public static IEnumerable<NamedAccountOption> GenerateInfiniteEncryptedAccountList(ICryptoAccountService cryptoAccount) {
        return AccountModelMocks.GenerateManyEncrypted(cryptoAccount).Select<AccountModel, NamedAccountOption>(x => x);
    }

    public static IAsyncEnumerable<NamedAccountOption> GenerateInfiniteEncryptedAccountListAsync(ICryptoAccountService cryptoAccount) {
        return AccountModelMocks.GenerateManyEncryptedAsync(cryptoAccount).Select<AccountModel, NamedAccountOption>(x => x);
    }

}
