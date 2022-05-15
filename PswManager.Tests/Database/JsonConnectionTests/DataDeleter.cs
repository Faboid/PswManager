using PswManager.Tests.Database.Generic;
using PswManager.Tests.Database.JsonConnectionTests.Helpers;

namespace PswManager.Tests.Database.JsonConnectionTests {
    public class DataDeleter : DataDeleterGeneric {

        public DataDeleter() : base(new JsonDBHandler("DataDeleterTests")) {

        }

    }
}
