using PswManagerTests.Database.MemoryConnectionTests.Helpers;
using PswManagerTests.Database.Generic;

namespace PswManagerTests.Database.MemoryConnectionTests {

    public class DataCreator : DataCreatorGeneric {

        public DataCreator() : base(new MemoryDBHandler(numValues)) { }

    }
}
