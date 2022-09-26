using PswManager.Database;
using PswManager.Database.Models;
using PswManager.Extensions;
using PswManager.TestUtils;

namespace PswManager.ConsoleUI.Tests.Commands.Helper;
internal class MemoryDBHandler {

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

    public MemoryDBHandler SetUpDefaultValues() {
        //reset database
        dbConnection.GetAllAccounts().Match(some => some, error => throw new System.Exception(), () => throw new System.Exception())
            .Select(x => x.Or(null) ?? throw new System.Exception())
            .ForEach(x => dbConnection.DeleteAccount(x.Name));

        Enumerable.Range(0, numValues).ForEach(x => {
            var name = defaultValues.GetValue(x, DefaultValues.TypeValue.Name);
            var password = defaultValues.GetValue(x, DefaultValues.TypeValue.Password);
            var email = defaultValues.GetValue(x, DefaultValues.TypeValue.Email);

            var model = new AccountModel(name, password, email);
            dbConnection.CreateAccountAsync(model).GetAwaiter().GetResult();
        });

        return this;
    }

    public IDataFactory GetDBFactory() {
        return factory;
    }
    public DefaultValues GetDefaultValues() {
        return defaultValues;
    }

}

