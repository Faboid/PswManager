using PswManager.Database;
using System;

namespace PswManager.Tests.Database.Generic {
    public interface ITestDBHandler : IDisposable {

        public ITestDBHandler SetUpDefaultValues();
        public IDataFactory GetDBFactory();
        public DefaultValues GetDefaultValues();

    }
}
