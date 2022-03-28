using PswManagerTests.Database.Generic;
using PswManagerTests.Database.SQLConnectionTests.Helpers;

namespace PswManagerTests.Database.SQLConnectionTests {
    public class DataHelper : DataHelperGeneric {

        public DataHelper() : base(new TestDatabaseHandler(db_Name, numValues)){ }
        const string db_Name = "DataHelperTestsDB";

    }
}
