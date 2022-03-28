using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerTests.Database.Generic;
using PswManagerTests.TestsHelpers;
using System;
using System.IO;

namespace PswManagerTests.Database.TextFileConnectionTests.Helpers {
    internal class TextDatabaseHandler : ITestDBHandler, IDisposable {

        public TextDatabaseHandler(string dbName) : this(dbName, 5) { }

        public TextDatabaseHandler(string dbName, int numValues) {
            DatabaseName = $"TextTestDB_{dbName}";
            dbFolder = Path.Combine(WorkingDirectory, "Data", DatabaseName);
            DefaultValues = new DefaultValues(numValues);
            paths = new CustomPaths(dbFolder);
            CreateFiles();
            factory = new DataFactory(DatabaseType.TextFile, paths);
            dataCreator = factory.GetDataCreator();
        }

        public DefaultValues DefaultValues { get; init; }
        private static readonly string WorkingDirectory = PswManagerHelperMethods.PathsBuilder.GetWorkingDirectory;
        private readonly string dbFolder;
        private readonly IDataCreator dataCreator;
        private readonly DataFactory factory;
        public readonly string DatabaseName;
        private readonly CustomPaths paths;

        public ITestDBHandler SetUpDefaultValues() {
            CreateFiles();

            foreach(var value in DefaultValues.values) {
                var account = DefaultValues.ToAccount(value);
                dataCreator.CreateAccount(account);
            }

            return this;
        }

        public IDataFactory GetDBFactory() {
            return factory;
        }

        private void CreateFiles() {
            Directory.CreateDirectory(paths.WorkingDirectory);
            File.Create(paths.AccountsFilePath).Close();
            File.Create(paths.PasswordsFilePath).Close();
            File.Create(paths.EmailsFilePath).Close();
        }

        public DefaultValues GetDefaultValues() {
            return DefaultValues;
        }

        public void Dispose() {
            Directory.Delete(paths.WorkingDirectory, true);
        }

    }
}
