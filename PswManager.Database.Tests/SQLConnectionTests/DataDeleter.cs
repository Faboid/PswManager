using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.SQLConnectionTests.Helpers;

namespace PswManager.Database.Tests.SQLConnectionTests;
public class DataDeleter : DataDeleterGeneric {

    public DataDeleter() : base(new TestDatabaseHandler(db_Name, numValues)) { }
    const string db_Name = "DataDeleterTestsDB";

}
