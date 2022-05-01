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
            var expectedAccounts = dbHandler.GetDefaultValues().GetAll().ToList();

            //act
            var actual = dataReader.GetAllAccounts();
            var values = actual.Value
                .Select(x => x.Value)
                .OrderBy(x => x.Name)
                .ToList();

            //assert
            Assert.True(actual.Success);
            Assert.Equal(expectedAccounts.Count, values.Count);

            Enumerable
                .Range(0, dbHandler.GetDefaultValues().values.Count - 1)
                .ForEach(x => {
                    AccountEqual(expectedAccounts[x], values[x]);
                });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetAFewWithIEnumerator(int num) {

            //arrange
            var expectedAccounts = dbHandler.GetDefaultValues().GetSome(num).ToList();

            //act
            var actual = dataReader.GetAllAccounts();
            var values = dataReader.GetAllAccounts()
                .Value
                .Select(x => x.Value)
                .OrderBy(x => x.Name)
                .Take(num)
                .ToList();

            //assert
            Assert.True(actual.Success);
            Assert.Equal(expectedAccounts.Count, values.Count);

            Enumerable
                .Range(0, num)
                .ForEach(x => {
                    AccountEqual(expectedAccounts[x], values[x]);
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
