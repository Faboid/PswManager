using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.MemoryConnectionTests.Helpers;

namespace PswManager.Database.Tests.MemoryConnectionTests;
public class DataDeleter : DataDeleterGeneric {

    public DataDeleter() : base(new MemoryDBHandler(numValues)) { }

}
