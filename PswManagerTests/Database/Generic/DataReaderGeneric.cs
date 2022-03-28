using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using System;
using System.Linq;
using Xunit;

namespace PswManagerTests.Database.Generic {
    public abstract class DataReaderGeneric : IDisposable {

        public DataReaderGeneric(ITestDBHandler dbHandler) {
            this.dbHandler = dbHandler.SetUpDefaultValues();
            dataReader = dbHandler.GetDBFactory().GetDataReader();
        }

        readonly IDataReader dataReader;
        readonly ITestDBHandler dbHandler;
        static protected readonly int numValues = 3;

        [Fact]
        public void GetOneShouldReturn() {

            //arrange
            AccountModel expected = new(
                dbHandler.GetDefaultValues().GetValue(1, TestsHelpers.DefaultValues.TypeValue.Name),
                dbHandler.GetDefaultValues().GetValue(1, TestsHelpers.DefaultValues.TypeValue.Password),
                dbHandler.GetDefaultValues().GetValue(1, TestsHelpers.DefaultValues.TypeValue.Email)
                );

            //act
            var actual = dataReader.GetAccount(expected.Name);

            //assert
            Assert.True(actual.Success);
            AccountEqual(expected, actual.Value);

        }

        [Fact]
        public void GetAllShouldGetAll() {

            //arrange
            var expectedAccounts = dbHandler.GetDefaultValues().GetAll();

            //act
            var actual = dataReader.GetAllAccounts();

            //assert
            Assert.True(actual.Success);
            Assert.Equal(expectedAccounts.Count, actual.Value.Count);

            Enumerable
                .Range(0, dbHandler.GetDefaultValues().values.Count - 1)
                .ForEach(x => {
                    AccountEqual(expectedAccounts[x], actual.Value[x]);
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
