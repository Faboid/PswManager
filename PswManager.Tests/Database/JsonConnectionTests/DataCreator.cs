using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.JsonConnectionTests.Helpers;

namespace PswManager.Tests.Database.JsonConnectionTests {
    public class DataCreator : DataCreatorGeneric {

        public DataCreator() : base(new JsonDBHandler("DataCreatorTests", numValues)) { }

    }
}
