using PswManager.Database.Tests.Generic;
using PswManager.Database.Tests.Mocks;

namespace PswManager.Database.Tests.SQLConnectionTests.Helpers;
internal class TestDatabaseHandler : ITestDBHandler, IDisposable {

    public TestDatabaseHandler(string dbName) : this(dbName, 5) { }

    public TestDatabaseHandler(string dbName, int numValues) {
        DatabaseName = $"SQLTestDB_{dbName}";
        _mockPathsBuilder = new MockPathsBuilder(DatabaseName);
        dbPath = _mockPathsBuilder.GetSQLDatabaseFile();
        defaultValues = new DefaultValues(numValues);
        dataFactory = new DataFactory(DatabaseType.Sql, _mockPathsBuilder);
    }

    private readonly MockPathsBuilder _mockPathsBuilder;
    private readonly DefaultValues defaultValues;
    private readonly string dbPath;
    private IDataFactory dataFactory;
    private readonly string DatabaseName;

    public ITestDBHandler SetUpDefaultValues() {
        //reset database
        File.Delete(dbPath);
        dataFactory = new DataFactory(DatabaseType.Sql, _mockPathsBuilder);

        foreach(var value in defaultValues.values) {
            var account = DefaultValues.ToAccount(value);
            dataFactory.GetDataCreator().CreateAccountAsync(account).GetAwaiter().GetResult();
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
