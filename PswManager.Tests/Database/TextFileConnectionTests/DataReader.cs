using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.TextFileConnectionTests.Helpers;

namespace PswManager.Tests.Database.TextFileConnectionTests {

    public class DataReader : DataReaderGeneric {

        public DataReader() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataReaderTestsDB";

    }
}
