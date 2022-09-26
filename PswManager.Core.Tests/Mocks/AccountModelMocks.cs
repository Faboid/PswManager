using PswManager.Core.Services;
using PswManager.Database.Models;
using PswManager.Extensions;

namespace PswManager.Core.Tests.Mocks;
public static class AccountModelMocks {

    public static AccountModel GenerateValidFromName(string name) {
        return new AccountModel {
            Name = name,
            Password = new string(name.Reverse().ToArray()),
            Email = Enumerable.Repeat(name, 3).JoinStrings("/")
        };
    }

    public static AccountModel GenerateEncryptedFromName(string name, ICryptoAccountService cryptoAccount) {
        return cryptoAccount.Encrypt(GenerateValidFromName(name));
    }

    public static async IAsyncEnumerable<AccountModel> GenerateManyEncryptedAsync(ICryptoAccountService cryptoAccount, int returns = int.MaxValue) {

        foreach(var result in GenerateManyEncrypted(cryptoAccount, returns)) {
            yield return await Task.FromResult(result); //kinda terrible, but it works(?)
        }
    }

    public static IEnumerable<AccountModel> GenerateManyEncrypted(ICryptoAccountService cryptoAccount, int returns = int.MaxValue) {

        char[] s = new string('0', 20).ToCharArray();
        int curr = 0;

        while(returns-- > 0) {
            yield return GenerateEncryptedFromName(new string(s), cryptoAccount);

            curr = (curr < 20)? curr++ : 0;
            switch((int)s[curr]) {
                case > 100:
                    s[curr] = '0';
                    break;
                default:
                    s[curr]++;
                    break;
            }
        }
    }

}
