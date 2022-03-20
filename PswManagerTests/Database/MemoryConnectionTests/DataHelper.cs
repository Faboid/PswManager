using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerTests.Database.MemoryConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Database.MemoryConnectionTests {
    public class DataHelper {

        public DataHelper() {
            dbHandler = new MemoryDBHandler();
            dataHelper = dbHandler
                .SetUpDefaultValues()
                .GetDBFactory()
                .GetDataHelper();
        }

        readonly IDataHelper dataHelper;
        readonly MemoryDBHandler dbHandler;


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

    }
}
