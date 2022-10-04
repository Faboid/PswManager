using PswManager.Extensions;
using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.Mocks;

namespace PswManager.Database.Tests.JsonConnectionTests.Helpers;
internal class JsonDBHandler : ITestDBHandler {

    public JsonDBHandler(string dbName) : this(dbName, 5) { }

    public JsonDBHandler(string dbName, int numValues) {
        DatabaseName = $"JsonTestsDB_{dbName}";
        var pathBuilderMock = new MockPathsBuilder(DatabaseName);
        dbPath = pathBuilderMock.GetJsonDatabaseDirectory();
        factory = new DataFactory(DatabaseType.Json, pathBuilderMock);
        defaultValues = new DefaultValues(numValues);
    }

    readonly string DatabaseName;
    readonly string dbPath;
    readonly IDataFactory factory;
    readonly DefaultValues defaultValues;

    public ITestDBHandler SetUpDefaultValues() {
        if(Directory.Exists(dbPath)) {
            Directory.GetFiles(dbPath)
                .ForEach(x => File.Delete(x));
        }

        foreach(var value in defaultValues.values) {
            var account = DefaultValues.ToAccount(value);
            factory.GetDataCreator().CreateAccountAsync(account).GetAwaiter().GetResult();
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
        Directory.Delete(dbPath, true);
    }

}
