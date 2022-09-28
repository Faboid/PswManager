using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.Database.Tests.Generic;

namespace PswManager.Database.Tests.MemoryConnectionTests.Helpers;
internal class MemoryDBHandler : ITestDBHandler {

    //constructs the class with a default value of five
    public MemoryDBHandler() : this(5) { }

    public MemoryDBHandler(int numDefaultValues) {
        numValues = numDefaultValues;
        factory = new DataFactory(DatabaseType.InMemory);
        dbConnection = factory.GetDataConnection();
        defaultValues = new(numValues);
    }

    readonly int numValues;
    readonly DefaultValues defaultValues;
    readonly DataFactory factory;
    readonly IDataConnection dbConnection;

    public ITestDBHandler SetUpDefaultValues() {
        return SetUpDefaultValuesAsync().GetAwaiter().GetResult();
    }

    public async Task<ITestDBHandler> SetUpDefaultValuesAsync() {
        //reset database
        await foreach(var acc in dbConnection.EnumerateAccountsAsync()) {
            await dbConnection.DeleteAccountAsync(acc.Match(some => some.Name, error => error.Name, () => throw new Exception()));
        }

        for(int i = 0; i < numValues; i++) {
            var name = defaultValues.GetValue(i, DefaultValues.TypeValue.Name);
            var password = defaultValues.GetValue(i, DefaultValues.TypeValue.Password);
            var email = defaultValues.GetValue(i, DefaultValues.TypeValue.Email);

            var model = new AccountModel(name, password, email);
            await dbConnection.CreateAccountAsync(model);
        }

        return this;
    }

    public IDataFactory GetDBFactory() {
        return factory;
    }
    public DefaultValues GetDefaultValues() {
        return defaultValues;
    }

    public void Dispose() { }

}
