using PswManagerTests.Database.Generic;
using PswManagerTests.Database.JsonConnectionTests.Helpers;

namespace PswManagerTests.Database.JsonConnectionTests {
    public class DataCreator : DataCreatorGeneric {

        public DataCreator() : base(new JsonDBHandler("DataCreatorTests")) { }

    }
}
