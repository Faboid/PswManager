using PswManagerTests.Database.Generic;
using PswManagerTests.Database.TextFileConnectionTests.Helpers;

namespace PswManagerTests.Database.TextFileConnectionTests {

    public class DataCreator : DataCreatorGeneric {

        public DataCreator() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataCreatorTestsDB";

    }
}
