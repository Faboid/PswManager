using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.JsonConnectionTests.Helpers;

namespace PswManager.Tests.Database.JsonConnectionTests {
    public class DataHelper : DataHelperGeneric {

        public DataHelper() : base(new JsonDBHandler("DataHelperTests", numValues)) {

        }

    }
}
