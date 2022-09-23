using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.JsonConnectionTests.Helpers;

namespace PswManager.Database.Tests.JsonConnectionTests; 
public class DataDeleter : DataDeleterGeneric {

    public DataDeleter() : base(new JsonDBHandler("DataDeleterTests")) {

    }

}
