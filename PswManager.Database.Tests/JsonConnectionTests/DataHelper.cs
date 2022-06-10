using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.JsonConnectionTests.Helpers;

namespace PswManager.Database.Tests.JsonConnectionTests {
    public class DataHelper : DataHelperGeneric {

        public DataHelper() : base(new JsonDBHandler("DataHelperTests", numValues)) {

        }

    }
}
