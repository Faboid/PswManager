using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.TextFileConnectionTests.Helpers;

namespace PswManager.Database.Tests.TextFileConnectionTests;
public class DataReader : DataReaderGeneric {

    public DataReader() : base(new TextDatabaseHandler(dbName, numValues)) { }
    const string dbName = "DataReaderTestsDB";

}
