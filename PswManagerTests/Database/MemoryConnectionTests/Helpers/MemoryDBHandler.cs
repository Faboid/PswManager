using PswManagerDatabase;
using PswManagerDatabase.DataAccess;
using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using PswManagerTests.TestsHelpers;
using System.Linq;

namespace PswManagerTests.Database.MemoryConnectionTests.Helpers {
    internal class MemoryDBHandler {

        public MemoryDBHandler() {
            factory = new DataFactory(DatabaseType.InMemory);
            dbConnection = factory.GetDataConnection();
            defaultValues = new(numValues);
        }

        readonly DataFactory factory;
        readonly IDataConnection dbConnection;
        public DefaultValues defaultValues;
        readonly int numValues = 5;

        public MemoryDBHandler SetUpDefaultValues() {
            //reset database
            var accounts = dbConnection.GetAllAccounts().Value;
            accounts.ForEach(x => dbConnection.DeleteAccount(x.Name));

            Enumerable.Range(0, numValues).ForEach(x => {
                var name = defaultValues.GetValue(x, DefaultValues.TypeValue.Name);
                var password = defaultValues.GetValue(x, DefaultValues.TypeValue.Password);
                var email = defaultValues.GetValue(x, DefaultValues.TypeValue.Email);

                var model = new AccountModel(name, password, email);
                dbConnection.CreateAccount(model);
            });

            return this;
        }

        public DataFactory GetDBFactory() {
            return factory;
        }

    }
}
