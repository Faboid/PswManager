using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Database.Generic {
    public abstract class DataHelperGeneric : IDisposable {

        public DataHelperGeneric(ITestDBHandler dbHandler) {
            this.dbHandler = dbHandler.SetUpDefaultValues();
            dataHelper = dbHandler.GetDBFactory().GetDataHelper();
        }

        readonly IDataHelper dataHelper;
        readonly ITestDBHandler dbHandler;
        protected static readonly int numValues = 1;

        public static IEnumerable<object[]> AccountExistTestsData() {
            static object[] NewObj(string name, bool shouldExist) => new object[] { name, shouldExist };

            yield return NewObj(DefaultValues.StaticGetValue(0, DefaultValues.TypeValue.Name), true);
            yield return NewObj("rtoehognrkljnwigurehvbonolbneughwonko", false);
            yield return NewObj("", false);
            yield return NewObj("   ", false);
            yield return NewObj(null, false);

        }

        [Theory]
        [MemberData(nameof(AccountExistTestsData))]
        public void AccountExist(string name, bool shouldExist) {

            Assert.True(dataHelper.AccountExist(name) == shouldExist);

        }

        [Theory]
        [MemberData(nameof(AccountExistTestsData))]
        public async Task AccountExistsAsync(string name, bool shouldExist) {
            Assert.True(await dataHelper.AccountExistAsync(name) == shouldExist);
        }

        public void Dispose() {
            dbHandler.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
