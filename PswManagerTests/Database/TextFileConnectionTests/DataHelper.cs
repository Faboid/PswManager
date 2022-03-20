using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerTests.Database.TextFileConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Database.TextFileConnectionTests {
    public class DataHelper : IDisposable {

        public DataHelper() {
            dbHandler = new TextDatabaseHandler(dbName, 1).SetUpDefaultValues();
            dataHelper = dbHandler.GetDBFactory().GetDataHelper();
        }

        readonly IDataHelper dataHelper;
        readonly TextDatabaseHandler dbHandler;
        const string dbName = "DataHelperTestsDB";

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
            Assert.True(dataHelper.AccountExist(name) == shouldExist);

        }

        public void Dispose() {
            dbHandler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
