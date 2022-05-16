using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.TextFileConnectionTests.Helpers;

namespace PswManager.Tests.Database.TextFileConnectionTests {

    public class DataCreator : DataCreatorGeneric {

        public DataCreator() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataCreatorTestsDB";

    }
}
