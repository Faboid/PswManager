using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.JsonConnectionTests.Helpers;

namespace PswManager.Database.Tests.JsonConnectionTests {
    public class DataReader : DataReaderGeneric {

        public DataReader() : base(new JsonDBHandler("DataReaderTests", numValues)) {

        }

    }
}
