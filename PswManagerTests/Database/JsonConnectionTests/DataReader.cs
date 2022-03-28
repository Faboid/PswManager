using PswManagerTests.Database.Generic;
using PswManagerTests.Database.JsonConnectionTests.Helpers;

namespace PswManagerTests.Database.JsonConnectionTests {
    public class DataReader : DataReaderGeneric {

        public DataReader() : base(new JsonDBHandler("DataReaderTests", numValues)) {

        }

    }
}
