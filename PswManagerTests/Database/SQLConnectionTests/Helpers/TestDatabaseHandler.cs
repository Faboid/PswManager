using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerTests.TestsHelpers;
using System;
using System.IO;

namespace PswManagerTests.Database.SQLConnectionTests.Helpers {
    internal class TestDatabaseHandler : IDisposable {

        public TestDatabaseHandler(string dbName) : this(dbName, 5) { }

        public TestDatabaseHandler(string dbName, int numValues) {
            DatabaseName = $"TestDB_{dbName}";
            dbPath = Path.Combine(WorkingDirectory, "Data", $"{DatabaseName}.db");
            defaultValues = new DefaultValues(numValues);
        }

        private static readonly string WorkingDirectory = PswManagerHelperMethods.PathsBuilder.GetWorkingDirectory;
        private readonly string dbPath;
        public IDataCreator dataCreator;
        public DefaultValues defaultValues;
        public string DatabaseName;

        public void SetUpDefaultValues() {
            //reset database
            File.Delete(dbPath);
            dataCreator = new DataFactory(DatabaseName).GetDataCreator();
            
            foreach(var value in defaultValues.values) {
                var account = DefaultValues.ToAccount(value);
                dataCreator.CreateAccount(account);
            }
        }

        public void Dispose() {
            File.Delete(dbPath);
        }
    }
}
