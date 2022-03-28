using PswManagerTests.Database.SQLConnectionTests.Helpers;
using PswManagerTests.Database.Generic;

namespace PswManagerTests.Database.SQLConnectionTests {

    public class DataEditor : DataEditorGeneric {

        public DataEditor() : base(new TestDatabaseHandler(db_Name, numValues)) { }
        const string db_Name = "DataEditorTestsDB";

    }
}
