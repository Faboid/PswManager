using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.TextFileConnectionTests.Helpers;

namespace PswManager.Database.Tests.TextFileConnectionTests;
public class DataCreator : DataCreatorGeneric {

    public DataCreator() : base(new TextDatabaseHandler(dbName, numValues)) { }
    const string dbName = "DataCreatorTestsDB";

}
