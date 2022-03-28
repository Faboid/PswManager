using PswManagerTests.Database.Generic;
using PswManagerTests.Database.JsonConnectionTests.Helpers;

namespace PswManagerTests.Database.JsonConnectionTests {
    public class DataHelper : DataHelperGeneric {

        public DataHelper() : base(new JsonDBHandler("DataHelperTests", numValues)) {

        }

    }
}
