using PswManagerDatabase.DataAccess;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using PswManagerLibrary.Storage;
using PswManagerTests.TestsHelpers;
using System.Linq;
using Xunit;

namespace PswManagerTests.Database.TextFileConnectionTests {

    [Collection("TestHelperCollection")]
    public class DataReader {

        public DataReader() {
            reader = new TextFileConnection(TestsHelper.Paths);
            TestsHelper.SetUpDefault();
        }

        readonly IDataReader reader;

        [Fact]
        public void GetOneShouldReturn() {

            //arrange
            AccountModel expected = new(
                TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name),
                TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Password),
                TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Email)
                );

            string name = TestsHelper.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name);

            //act
            var actual = reader.GetAccount(name);
            DecryptAccountValues(actual.Value);

            //assert
            AccountEqual(expected, actual.Value);

        }

        [Fact]
        public void GetAllShouldGetAll() {

            //arrange
            var expectedAccounts = TestsHelper.DefaultValues.GetAll();

            //act
            var actual = reader.GetAllAccounts().Value;
            actual.ForEach(DecryptAccountValues);

            //assert
            Assert.Equal(expectedAccounts.Count, actual.Count);

            Enumerable
                .Range(0, TestsHelper.DefaultValues.values.Count - 1)
                .ForEach(i => {
                    AccountEqual(expectedAccounts[i], actual[i]);
                });
        }

        private void AccountEqual(AccountModel expected, AccountModel actual) {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Password, actual.Password);
            Assert.Equal(expected.Email, actual.Email);
        }

        private void DecryptAccountValues(AccountModel account) {
            (account.Password, account.Email) = TestsHelper.CryptoAccount.Decrypt(account.Password, account.Email);
        }

    }
}
