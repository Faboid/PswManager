using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.TextFileConnectionTests.Helpers;

namespace PswManager.Database.Tests.TextFileConnectionTests;
public class DataEditor : DataEditorGeneric {

    public DataEditor() : base(new TextDatabaseHandler(dbName, numValues)) { }
    const string dbName = "DataEditorTestsDB";

}
