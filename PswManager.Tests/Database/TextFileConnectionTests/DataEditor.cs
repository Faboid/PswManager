using PswManager.Tests.Database.TextFileConnectionTests.Helpers;
using PswManager.Tests.Database.Generic;

namespace PswManager.Tests.Database.TextFileConnectionTests {

    public class DataEditor : DataEditorGeneric {

        public DataEditor() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataEditorTestsDB";

    }
}
