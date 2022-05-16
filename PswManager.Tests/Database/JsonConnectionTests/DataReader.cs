using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.JsonConnectionTests.Helpers;

namespace PswManager.Tests.Database.JsonConnectionTests {
    public class DataReader : DataReaderGeneric {

        public DataReader() : base(new JsonDBHandler("DataReaderTests", numValues)) {

        }

    }
}
