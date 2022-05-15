using PswManager.Database.DataAccess.Interfaces;
using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.TextFileConnectionTests.Helpers;
using PswManager.Tests.TestsHelpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PswManager.Tests.Database.TextFileConnectionTests {
    public class DataHelper : DataHelperGeneric {

        public DataHelper() : base(new TextDatabaseHandler(dbName, numValues)) { }
        const string dbName = "DataHelperTestsDB";

    }
}
