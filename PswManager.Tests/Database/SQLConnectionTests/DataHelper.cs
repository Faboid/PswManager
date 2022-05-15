using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.SQLConnectionTests.Helpers;

namespace PswManager.Tests.Database.SQLConnectionTests {
    public class DataHelper : DataHelperGeneric {

        public DataHelper() : base(new TestDatabaseHandler(db_Name, numValues)){ }
        const string db_Name = "DataHelperTestsDB";

    }
}
