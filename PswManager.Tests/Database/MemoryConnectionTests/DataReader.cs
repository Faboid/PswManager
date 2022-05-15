using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.MemoryConnectionTests.Helpers;

namespace PswManager.Tests.Database.MemoryConnectionTests {

    public class DataReader : DataReaderGeneric {

        public DataReader() : base(new MemoryDBHandler(numValues)) { }

    }
}
