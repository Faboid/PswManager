using PswManager.Tests.Database.MemoryConnectionTests.Helpers;
using PswManager.Tests.Database.Generic;

namespace PswManager.Tests.Database.MemoryConnectionTests {

    public class DataEditor : DataEditorGeneric {

        public DataEditor() : base(new MemoryDBHandler(numValues)) { }

    }
}
