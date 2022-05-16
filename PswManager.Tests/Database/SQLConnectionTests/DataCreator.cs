using PswManager.Tests.Database.SQLConnectionTests.Helpers;
using PswManager.Tests.Database.Generic;

namespace PswManager.Tests.Database.SQLConnectionTests {

    public class DataCreator : DataCreatorGeneric {

        public DataCreator() : base(new TestDatabaseHandler(db_Name, numValues)) {}
        const string db_Name = "DataCreatorTestsDB";

    }
}
