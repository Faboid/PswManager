using PswManagerDatabase;
using PswManagerTests.TestsHelpers;
using System;

namespace PswManagerTests.Database.Generic {
    public interface ITestDBHandler : IDisposable {

        public ITestDBHandler SetUpDefaultValues();
        public IDataFactory GetDBFactory();
        public DefaultValues GetDefaultValues();

    }
}
