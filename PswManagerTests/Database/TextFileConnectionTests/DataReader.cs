using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using PswManagerTests.Database.TextFileConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System;
using System.Linq;
using Xunit;

namespace PswManagerTests.Database.TextFileConnectionTests {

    public class DataReader : IDisposable {

        public DataReader() {
            dbHandler = new TextDatabaseHandler(dbName, numValues).SetUpDefaultValues();
            reader = dbHandler.GetDBFactory().GetDataReader();
        }

        readonly IDataReader reader;
        readonly TextDatabaseHandler dbHandler;
        const string dbName = "DataReaderTestsDB";
        readonly int numValues = 3;

        [Fact]
        public void GetOneShouldReturn() {

            //arrange
            AccountModel expected = new(
                dbHandler.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name),
                dbHandler.DefaultValues.GetValue(1, DefaultValues.TypeValue.Password),
                dbHandler.DefaultValues.GetValue(1, DefaultValues.TypeValue.Email)
                );

            string name = dbHandler.DefaultValues.GetValue(1, DefaultValues.TypeValue.Name);

            //act
            var actual = reader.GetAccount(name);

            //assert
            AccountEqual(expected, actual.Value);

        }

        [Fact]
        public void GetAllShouldGetAll() {

            //arrange
            var expectedAccounts = new DefaultValues(numValues).GetAll();

            //act
            var actual = reader.GetAllAccounts().Value;

            //assert
            Assert.Equal(expectedAccounts.Count, actual.Count);

            Enumerable
                .Range(0, numValues)
                .ForEach(i => {
                    AccountEqual(expectedAccounts[i], actual[i]);
                });
        }

        private static void AccountEqual(AccountModel expected, AccountModel actual) {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Password, actual.Password);
            Assert.Equal(expected.Email, actual.Email);
        }

        public void Dispose() {
            dbHandler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
