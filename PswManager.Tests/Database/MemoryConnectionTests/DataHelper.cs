using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.MemoryConnectionTests.Helpers;

namespace PswManager.Tests.Database.MemoryConnectionTests {
    public class DataHelper : DataHelperGeneric {

        public DataHelper() : base(new MemoryDBHandler(numValues)) { }

    }
}
