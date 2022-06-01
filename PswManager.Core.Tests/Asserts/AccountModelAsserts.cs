using PswManager.Database.Models;
using Xunit;

namespace PswManager.Core.Tests.Asserts {
    internal class AccountModelAsserts {

        public static bool AssertEqual(AccountModel expected, AccountModel actual) {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Password, actual.Password);
            Assert.Equal(expected.Email, actual.Email);
            return true;
        }

    }
}
