using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using PswManagerTests.Database.MemoryConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System.Linq;
using Xunit;

namespace PswManagerTests.Database.MemoryConnectionTests {

    public class DataReader {

        public DataReader() {
            dbHandler = new MemoryDBHandler();
            reader = dbHandler
                .SetUpDefaultValues()
                .GetDBFactory()
                .GetDataReader();
        }

        readonly IDataReader reader;
        readonly MemoryDBHandler dbHandler;

        [Fact]
        public void GetOneShouldReturn() {

            //arrange
            dbHandler.SetUpDefaultValues();
            AccountModel expected = new(
                dbHandler.defaultValues.GetValue(1, DefaultValues.TypeValue.Name),
                dbHandler.defaultValues.GetValue(1, DefaultValues.TypeValue.Password),
                dbHandler.defaultValues.GetValue(1, DefaultValues.TypeValue.Email)
                );

            //act
            var actual = reader.GetAccount(expected.Name);

            //assert
            AccountEqual(expected, actual.Value);

        }

        [Fact]
        public void GetAllShouldGetAll() {

            //arrange
            var expectedAccounts = dbHandler.defaultValues.GetAll();

            //act
            var actual = reader.GetAllAccounts().Value;

            //assert
            Assert.Equal(expectedAccounts.Count, actual.Count);

            Enumerable
                .Range(0, dbHandler.defaultValues.values.Count - 1)
                .ForEach(i => {
                    AccountEqual(expectedAccounts[i], actual[i]);
                });
        }

        private static void AccountEqual(AccountModel expected, AccountModel actual) {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Password, actual.Password);
            Assert.Equal(expected.Email, actual.Email);
        }

    }
}
