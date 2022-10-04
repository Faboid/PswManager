using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.MemoryConnectionTests.Helpers;

namespace PswManager.Database.Tests.MemoryConnectionTests;
public class DataEditor : DataEditorGeneric {

    public DataEditor() : base(new MemoryDBHandler(numValues)) { }

}
