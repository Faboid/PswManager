using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.MemoryConnectionTests.Helpers;

namespace PswManager.Database.Tests.MemoryConnectionTests;
public class DataReader : DataReaderGeneric {

    public DataReader() : base(new MemoryDBHandler(numValues)) { }

}
