using PswManagerTests.Database.Generic;
using PswManagerTests.Database.MemoryConnectionTests.Helpers;

namespace PswManagerTests.Database.MemoryConnectionTests {
    public class DataHelper : DataHelperGeneric {

        public DataHelper() : base(new MemoryDBHandler(numValues)) { }

    }
}
