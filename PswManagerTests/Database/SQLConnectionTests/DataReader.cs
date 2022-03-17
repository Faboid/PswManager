using PswManagerDatabase;
using PswManagerDatabase.DataAccess;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using PswManagerLibrary.Storage;
using PswManagerTests.Database.SQLConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System.Linq;
using Xunit;

namespace PswManagerTests.Database.SQLConnectionTests {

    public class DataReader {

        public DataReader() {
            dbHandler = new TestDatabaseHandler(db_Name);
            IDataFactory dataFactory = new DataFactory(dbHandler.DatabaseName);
            reader = dataFactory.GetDataReader();
        }

        const string db_Name = "DataReaderTestsDB";
        readonly IDataReader reader;
        readonly TestDatabaseHandler dbHandler;

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
            dbHandler.SetUpDefaultValues();
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
