using PswManager.Database;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Utils;
using PswManager.Tests.Database.Generic;
using System;
using System.IO;

namespace PswManager.Tests.Database.TextFileConnectionTests.Helpers {
    internal class TextDatabaseHandler : ITestDBHandler, IDisposable {

        public TextDatabaseHandler(string dbName) : this(dbName, 5) { }

        public TextDatabaseHandler(string dbName, int numValues) {
            DatabaseName = $"TextTestDB_{dbName}";
            defaultValues = new DefaultValues(numValues);
            factory = new DataFactory(DatabaseType.TextFile, DatabaseName);
            dataCreator = factory.GetDataCreator();
            folderDB = Path.Combine(PathsBuilder.GetWorkingDirectory, "Data", DatabaseName);
        }

        private readonly DefaultValues defaultValues;
        private readonly IDataCreator dataCreator;
        private readonly DataFactory factory;
        private readonly string DatabaseName;
        private readonly string folderDB;

        public ITestDBHandler SetUpDefaultValues() {
            foreach(var value in defaultValues.values) {
                var account = DefaultValues.ToAccount(value);
                dataCreator.CreateAccount(account);
            }

            return this;
        }

        public IDataFactory GetDBFactory() {
            return factory;
        }

        public DefaultValues GetDefaultValues() {
            return defaultValues;
        }

        public void Dispose() {
            Directory.Delete(folderDB, true);
        }

    }
}
