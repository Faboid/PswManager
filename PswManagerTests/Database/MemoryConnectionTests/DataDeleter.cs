using PswManagerTests.Database.MemoryConnectionTests.Helpers;
using PswManagerTests.Database.Generic;

namespace PswManagerTests.Database.MemoryConnectionTests {

    public class DataDeleter : DataDeleterGeneric {

        public DataDeleter() : base(new MemoryDBHandler(numValues)) { }

    }
}
