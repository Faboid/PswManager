namespace PswManager.Database.Tests.Generic {
    public interface ITestDBHandler : IDisposable {

        public ITestDBHandler SetUpDefaultValues();
        public IDataFactory GetDBFactory();
        public DefaultValues GetDefaultValues();

    }
}
