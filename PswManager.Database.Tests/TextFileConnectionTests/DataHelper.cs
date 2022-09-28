using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.TextFileConnectionTests.Helpers;

namespace PswManager.Database.Tests.TextFileConnectionTests;
public class DataHelper : DataHelperGeneric {

    public DataHelper() : base(new TextDatabaseHandler(dbName, numValues)) { }
    const string dbName = "DataHelperTestsDB";

}
