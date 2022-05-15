using PswManager.Tests.Database.TextFileConnectionTests.Helpers;
using PswManager.Tests.Database.Generic;

namespace PswManager.Tests.Database.TextFileConnectionTests {

    public class DataDeleter : DataDeleterGeneric {

        public DataDeleter() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataDeleterTestsDB";

    }
}
