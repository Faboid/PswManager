using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.MemoryConnectionTests.Helpers;

namespace PswManager.Database.Tests.MemoryConnectionTests; 
public class DataCreator : DataCreatorGeneric {

    public DataCreator() : base(new MemoryDBHandler(numValues)) { }

}
