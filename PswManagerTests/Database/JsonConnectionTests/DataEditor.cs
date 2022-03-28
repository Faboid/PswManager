using PswManagerTests.Database.Generic;
using PswManagerTests.Database.JsonConnectionTests.Helpers;

namespace PswManagerTests.Database.JsonConnectionTests {
    public class DataEditor : DataEditorGeneric {

        public DataEditor() : base(new JsonDBHandler("DataEditorTests", numValues)) { }

    }
}
