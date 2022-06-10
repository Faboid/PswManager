using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.JsonConnectionTests.Helpers;

namespace PswManager.Database.Tests.JsonConnectionTests {
    public class DataCreator : DataCreatorGeneric {

        public DataCreator() : base(new JsonDBHandler("DataCreatorTests", numValues)) { }

    }
}
