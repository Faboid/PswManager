using PswManagerTests.Database.MemoryConnectionTests.Helpers;
using PswManagerTests.Database.Generic;

namespace PswManagerTests.Database.MemoryConnectionTests {

    public class DataEditor : DataEditorGeneric {

        public DataEditor() : base(new MemoryDBHandler(numValues)) { }

    }
}
