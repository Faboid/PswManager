using PswManagerDatabase;
using PswManagerTests.Database.Generic;
using PswManagerTests.TestsHelpers;
using System;
using System.IO;

namespace PswManagerTests.Database.SQLConnectionTests.Helpers {
    internal class TestDatabaseHandler : ITestDBHandler, IDisposable {

        public TestDatabaseHandler(string dbName) : this(dbName, 5) { }

        public TestDatabaseHandler(string dbName, int numValues) {
            DatabaseName = $"SQLTestDB_{dbName}";
            dbPath = Path.Combine(WorkingDirectory, "Data", $"{DatabaseName}.db");
            DefaultValues = new DefaultValues(numValues);
            dataFactory = new DataFactory(DatabaseType.Sql, DatabaseName);
        }

        public DefaultValues DefaultValues { get; init; }

        private static readonly string WorkingDirectory = PswManagerHelperMethods.PathsBuilder.GetWorkingDirectory;
        private readonly string dbPath;
        private IDataFactory dataFactory;
        public readonly string DatabaseName;

        public ITestDBHandler SetUpDefaultValues() {
            //reset database
            File.Delete(dbPath);
            dataFactory = new DataFactory(DatabaseType.Sql, DatabaseName);
            
            foreach(var value in DefaultValues.values) {
                var account = DefaultValues.ToAccount(value);
                dataFactory.GetDataCreator().CreateAccount(account);
            }

            return this;
        }

        public IDataFactory GetDBFactory() {
            return dataFactory;
        }

        public DefaultValues GetDefaultValues() {
            return DefaultValues;
        }

        public void Dispose() {
            File.Delete(dbPath);
        }

    }
}
