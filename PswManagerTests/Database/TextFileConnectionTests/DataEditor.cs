using PswManagerTests.Database.TextFileConnectionTests.Helpers;
using PswManagerTests.Database.Generic;

namespace PswManagerTests.Database.TextFileConnectionTests {

    public class DataEditor : DataEditorGeneric {

        public DataEditor() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataEditorTestsDB";

    }
}
