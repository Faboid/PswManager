using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.JsonConnectionTests.Helpers;

namespace PswManager.Database.Tests.JsonConnectionTests;
public class DataEditor : DataEditorGeneric {

    public DataEditor() : base(new JsonDBHandler("DataEditorTests", numValues)) { }

}
