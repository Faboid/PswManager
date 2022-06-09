using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.TextFileConnectionTests.Helpers;

namespace PswManager.Tests.Database.TextFileConnectionTests {
    public class DataHelper : DataHelperGeneric {

        public DataHelper() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataHelperTestsDB";

    }
}
