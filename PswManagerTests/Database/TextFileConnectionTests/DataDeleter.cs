using PswManagerTests.Database.TextFileConnectionTests.Helpers;
using PswManagerTests.Database.Generic;

namespace PswManagerTests.Database.TextFileConnectionTests {

    public class DataDeleter : DataDeleterGeneric {

        public DataDeleter() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataDeleterTestsDB";

    }
}
