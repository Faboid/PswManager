using PswManager.Core.Services;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;

namespace PswManager.Core.Tests.Mocks;
internal class OptionMocks {

    public static Option<CreatorErrorCode> ValidateValues(AccountModel model) {

        if(string.IsNullOrWhiteSpace(model.Name)) return CreatorErrorCode.InvalidName;
        if(string.IsNullOrWhiteSpace(model.Password)) return CreatorErrorCode.MissingPassword;
        if(string.IsNullOrWhiteSpace(model.Email)) return CreatorErrorCode.MissingEmail;

        return Option.None<CreatorErrorCode>();
    }

    public static Option<IEnumerable<NamedAccountOption>, ReaderAllErrorCode> GenerateInfiniteEncryptedAccountList(ICryptoAccountService cryptoAccount) {
        return new(AccountModelMocks.GenerateManyEncrypted(cryptoAccount).Select<AccountModel, NamedAccountOption>(x => x));
    }

    public static Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode> GenerateInfiniteEncryptedAccountListAsync(ICryptoAccountService cryptoAccount) {
        return new(AccountModelMocks.GenerateManyEncryptedAsync(cryptoAccount).Select<AccountModel, NamedAccountOption>(x => x));
    }

}
