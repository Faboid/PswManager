using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.TextFileConnectionTests.Helpers;

namespace PswManager.Database.Tests.TextFileConnectionTests {

    public class DataDeleter : DataDeleterGeneric {

        public DataDeleter() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataDeleterTestsDB";

    }
}
