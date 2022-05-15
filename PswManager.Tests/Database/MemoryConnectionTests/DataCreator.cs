using PswManager.Tests.Database.MemoryConnectionTests.Helpers;
using PswManager.Tests.Database.Generic;

namespace PswManager.Tests.Database.MemoryConnectionTests {

    public class DataCreator : DataCreatorGeneric {

        public DataCreator() : base(new MemoryDBHandler(numValues)) { }

    }
}
