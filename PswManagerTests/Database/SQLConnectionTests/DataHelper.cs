using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerTests.Database.SQLConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Database.SQLConnectionTests {
    public class DataHelper : IDisposable {

        public DataHelper() {
            dbHandler = new TestDatabaseHandler(db_Name);
            IDataFactory dataFactory = new DataFactory(dbHandler.DatabaseName);
            dataHelper = dataFactory.GetDataHelper();
        }

        const string db_Name = "DataHelperTestsDB";
        readonly IDataHelper dataHelper;
        readonly TestDatabaseHandler dbHandler;

        public static IEnumerable<object[]> AccountExistTestsData() {
            static object[] NewObj(string accName, bool shouldExist) => new object[] { accName, shouldExist };

            yield return NewObj(DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name), true);
            yield return NewObj("rtoehognrkljnwigurehvbonolbneughwonko", false);
            yield return NewObj("", false);
            yield return NewObj("   ", false);
            yield return NewObj(null, false);

        }

        [Theory]
        [MemberData(nameof(AccountExistTestsData))]
        public void AccountExist(string name, bool shouldExist) {

            //assert
            dbHandler.SetUpDefaultValues();
            Assert.True(dataHelper.AccountExist(name) == shouldExist);

        }

        public void Dispose() {
            dbHandler.Dispose();
        }
    }
}
