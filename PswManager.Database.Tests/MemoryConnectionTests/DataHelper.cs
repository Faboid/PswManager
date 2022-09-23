using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.MemoryConnectionTests.Helpers;

namespace PswManager.Database.Tests.MemoryConnectionTests; 
public class DataHelper : DataHelperGeneric {

    public DataHelper() : base(new MemoryDBHandler(numValues)) { }

}
