using PswManagerDatabase;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerTests.TestsHelpers;
using System;
using System.IO;

namespace PswManagerTests.Database.SQLConnectionTests.Helpers {
    internal class TestDatabaseHandler : IDisposable {

        public TestDatabaseHandler(string dbName) {
            DatabaseName = $"TestDB_{dbName}";
            dbPath = Path.Combine(WorkingDirectory, "Data", $"{DatabaseName}.db");
            defaultValues = new DefaultValues(5);
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

            for(int i = 0; i < defaultValues.values.Count; i++) {
                var name = defaultValues.GetValue(i, DefaultValues.TypeValue.Name);
                var password = defaultValues.GetValue(i, DefaultValues.TypeValue.Password);
                var email = defaultValues.GetValue(i, DefaultValues.TypeValue.Email);

                var model = new AccountModel(name, password, email);
                dataCreator.CreateAccount(model);
            }
        }

        public void Dispose() {
            File.Delete(dbPath);
        }
    }
}
