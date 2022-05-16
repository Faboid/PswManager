using PswManager.Tests.Database.MemoryConnectionTests.Helpers;
using PswManager.Tests.Database.Generic;

namespace PswManager.Tests.Database.MemoryConnectionTests {

    public class DataDeleter : DataDeleterGeneric {

        public DataDeleter() : base(new MemoryDBHandler(numValues)) { }

    }
}
