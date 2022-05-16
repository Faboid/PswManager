using PswManager.Tests.Database.SQLConnectionTests.Helpers;
using PswManager.Tests.Database.Generic;

namespace PswManager.Tests.Database.SQLConnectionTests {

    public class DataDeleter : DataDeleterGeneric {

        public DataDeleter() : base(new TestDatabaseHandler(db_Name, numValues)) { }
        const string db_Name = "DataDeleterTestsDB";

    }
}
