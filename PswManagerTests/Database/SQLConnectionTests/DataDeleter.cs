using PswManagerTests.Database.SQLConnectionTests.Helpers;
using PswManagerTests.Database.Generic;

namespace PswManagerTests.Database.SQLConnectionTests {

    public class DataDeleter : DataDeleterGeneric {

        public DataDeleter() : base(new TestDatabaseHandler(db_Name, numValues)) { }
        const string db_Name = "DataDeleterTestsDB";

    }
}
