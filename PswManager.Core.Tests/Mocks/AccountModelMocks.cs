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

    }
}
