using PswManagerTests.Database.Generic;
using PswManagerTests.Database.TextFileConnectionTests.Helpers;

namespace PswManagerTests.Database.TextFileConnectionTests {

    public class DataReader : DataReaderGeneric {

        public DataReader() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataReaderTestsDB";

    }
}
