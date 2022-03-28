using PswManagerTests.Database.Generic;
using PswManagerTests.Database.JsonConnectionTests.Helpers;

namespace PswManagerTests.Database.JsonConnectionTests {
    public class DataDeleter : DataDeleterGeneric {

        public DataDeleter() : base(new JsonDBHandler("DataDeleterTests")) {

        }

    }
}
