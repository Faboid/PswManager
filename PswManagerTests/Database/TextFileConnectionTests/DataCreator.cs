using PswManagerDatabase;
using PswManagerDatabase.DataAccess;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Database.TextFileConnectionTests {

    [Collection("TestHelperCollection")]
    public class DataCreator {

        public DataCreator() {
            IDataFactory dataFactory = new DataFactory(TestsHelper.Paths);
            dataCreator = dataFactory.GetDataCreator();
        }

        readonly IDataCreator dataCreator;

        [Fact]
        public void CreateAccountCorrectly() {

            //arrange
            AccountModel account = new AccountModel("newLovelyAccount", "girhwugrrigjth", "eco@email.yo");

            //act
            EncryptAccountValues(account);

            bool exist = dataCreator.AccountExist(account.Name);
            var result = dataCreator.CreateAccount(account);
            
            //assert
            Assert.False(exist);
            Assert.True(result.Success);
            Assert.True(dataCreator.AccountExist(account.Name));

        }

        [Fact]
        public void CreateAccountFailure_AlreadyExists() {

            //arrange
            AccountModel account = new AccountModel(TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name), "password", "email");

            //act
            EncryptAccountValues(account);
            bool exist = dataCreator.AccountExist(account.Name);
            var result = dataCreator.CreateAccount(account);

            //assert
            Assert.True(exist);
            Assert.False(result.Success);

        }

        private static void EncryptAccountValues(AccountModel account) {
            (account.Password, account.Email) = TestsHelper.CryptoAccount.Encrypt(account.Password, account.Email);
        }

    }
}
