using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.JsonConnectionTests.Helpers;

namespace PswManager.Tests.Database.JsonConnectionTests {
    public class DataEditor : DataEditorGeneric {

        public DataEditor() : base(new JsonDBHandler("DataEditorTests", numValues)) { }

    }
}
