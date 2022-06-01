using PswManager.Core.Cryptography;
using PswManager.Database.Models;
using PswManager.Utils;

namespace PswManager.Core.Tests.Mocks {
    internal static class AccountModelMocks {

        public static AccountModel GenerateValidFromName(string name) {
            return new AccountModel {
                Name = name,
                Password = new string(name.Reverse().ToArray()),
                Email = Enumerable.Repeat(name, 3).JoinStrings("/")
            };
        }

        public static AccountModel GenerateEncryptedFromName(string name, ICryptoAccount cryptoAccount) {
            return cryptoAccount.Encrypt(GenerateValidFromName(name));
        }

        public static async IAsyncEnumerable<AccountModel> GenerateManyAsync(int returns = int.MaxValue) {

            foreach(var result in GenerateMany(returns)) {
                yield return await Task.FromResult(result); //kinda terrible, but it works(?)
            }
        }

        public static IEnumerable<AccountModel> GenerateMany(int returns = int.MaxValue) {

            Random random = new();
            char[] s = new string('0', 20).ToCharArray();

            while(returns-- > 0) {
                yield return GenerateValidFromName(new string(s));

                var val = random.Next(0, 20);
                switch((int)s[val]) {
                    case > 100:
                        s[val] = '0';
                        break;
                    default:
                        s[val]++;
                        break;
                }
            }
        }

    }
}
