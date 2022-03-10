using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerTests.TestsHelpers;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Database.TextFileConnectionTests {
    [Collection("TestHelperCollection")]
    public class DataHelper {

        public DataHelper() {
            IDataFactory dataFactory = new DataFactory(TestsHelper.Paths);
            dataHelper = dataFactory.GetDataHelper();
            TestsHelper.SetUpDefault();
        }

        readonly IDataHelper dataHelper;

        public static IEnumerable<object[]> AccountExistTestsData() { 
            static object[] NewObj(string accName, bool shouldExist) => new object[] { accName, shouldExist };

            yield return NewObj(TestsHelper.DefaultValues.GetValue(0, DefaultValues.TypeValue.Name), true);
            yield return NewObj("rtoehognrkljnwigurehvbonolbneughwonko", false);
            yield return NewObj("", false);
            yield return NewObj(null, false);

        }

        [Theory]
        [MemberData(nameof(AccountExistTestsData))]
        public void AccountExist(string name, bool shouldExist) {

            //assert
            Assert.True(dataHelper.AccountExist(name) == shouldExist);

        }
    
    }
}
