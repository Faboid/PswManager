using PswManager.Utils;
using PswManager.Database.Tests.Generic;

namespace PswManager.Database.Tests.SQLConnectionTests.Helpers {
    internal class TestDatabaseHandler : ITestDBHandler, IDisposable {

        public TestDatabaseHandler(string dbName) : this(dbName, 5) { }

        public TestDatabaseHandler(string dbName, int numValues) {
            DatabaseName = $"SQLTestDB_{dbName}";
            dbPath = Path.Combine(PathsBuilder.GetWorkingDirectory, "Data", $"{DatabaseName}.db");
            defaultValues = new DefaultValues(numValues);
            dataFactory = new DataFactory(DatabaseType.Sql, DatabaseName);
        }

        private readonly DefaultValues defaultValues;
        private readonly string dbPath;
        private IDataFactory dataFactory;
        private readonly string DatabaseName;

        public ITestDBHandler SetUpDefaultValues() {
            //reset database
            File.Delete(dbPath);
            dataFactory = new DataFactory(DatabaseType.Sql, DatabaseName);

            foreach(var value in defaultValues.values) {
                var account = DefaultValues.ToAccount(value);
                dataFactory.GetDataCreator().CreateAccount(account);
            }

            return this;
        }

        public IDataFactory GetDBFactory() {
            return dataFactory;
        }

        public DefaultValues GetDefaultValues() {
            return defaultValues;
        }

        public void Dispose() {
            File.Delete(dbPath);
        }

    }
}
