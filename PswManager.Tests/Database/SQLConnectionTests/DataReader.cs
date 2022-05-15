using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.SQLConnectionTests.Helpers;

namespace PswManager.Tests.Database.SQLConnectionTests {

    public class DataReader : DataReaderGeneric {

        public DataReader() : base(new TestDatabaseHandler(db_Name, numValues)) { }
        const string db_Name = "DataReaderTestsDB";

    }
}
