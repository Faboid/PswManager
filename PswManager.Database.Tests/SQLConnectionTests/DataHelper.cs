using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.SQLConnectionTests.Helpers;

namespace PswManager.Database.Tests.SQLConnectionTests; 
public class DataHelper : DataHelperGeneric {

    public DataHelper() : base(new TestDatabaseHandler(db_Name, numValues)) { }
    const string db_Name = "DataHelperTestsDB";

}
