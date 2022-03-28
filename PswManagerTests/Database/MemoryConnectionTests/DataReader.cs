using PswManagerTests.Database.Generic;
using PswManagerTests.Database.MemoryConnectionTests.Helpers;

namespace PswManagerTests.Database.MemoryConnectionTests {

    public class DataReader : DataReaderGeneric {

        public DataReader() : base(new MemoryDBHandler(numValues)) { }

    }
}
