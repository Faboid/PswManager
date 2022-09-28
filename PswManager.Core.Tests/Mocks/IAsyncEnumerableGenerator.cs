using PswManager.Core.Services;
using PswManager.Database.Models;

namespace PswManager.Core.Tests.Mocks;

public class IAsyncEnumerableGenerator {

    public static IEnumerable<NamedAccountOption> GenerateInfiniteEncryptedAccountList(ICryptoAccountService cryptoAccount) {
        return AccountModelMocks.GenerateManyEncrypted(cryptoAccount).Select<IAccountModel, NamedAccountOption>(x => new(x));
    }

    public static IAsyncEnumerable<NamedAccountOption> GenerateInfiniteEncryptedAccountListAsync(ICryptoAccountService cryptoAccount) {
        return AccountModelMocks.GenerateManyEncryptedAsync(cryptoAccount).Select<IAccountModel, NamedAccountOption>(x => new(x));
    }

}
