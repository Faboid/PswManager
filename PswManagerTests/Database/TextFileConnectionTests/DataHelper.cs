using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerTests.Database.Generic;
using PswManagerTests.Database.TextFileConnectionTests.Helpers;
using PswManagerTests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManagerTests.Database.TextFileConnectionTests {
    public class DataHelper : DataHelperGeneric {

        public DataHelper() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataHelperTestsDB";

    }
}
