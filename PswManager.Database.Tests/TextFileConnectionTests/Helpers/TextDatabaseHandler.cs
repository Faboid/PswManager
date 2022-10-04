using PswManager.Database.Tests.Generic;
using PswManager.Database.Interfaces;
using PswManager.Database.Tests.Mocks;

namespace PswManager.Database.Tests.TextFileConnectionTests.Helpers;
internal class TextDatabaseHandler : ITestDBHandler, IDisposable {

    public TextDatabaseHandler(string dbName) : this(dbName, 5) { }

    public TextDatabaseHandler(string dbName, int numValues) {
        DatabaseName = $"TextTestDB_{dbName}";
        var mockPathsBuilder = new MockPathsBuilder(DatabaseName);
        defaultValues = new DefaultValues(numValues);
        factory = new DataFactory(DatabaseType.TextFile, mockPathsBuilder);
        dataCreator = factory.GetDataCreator();
        folderDB = mockPathsBuilder.GetTextDatabaseDirectory();
    }

    private readonly DefaultValues defaultValues;
    private readonly IDataCreator dataCreator;
    private readonly DataFactory factory;
    private readonly string DatabaseName;
    private readonly string folderDB;

    public ITestDBHandler SetUpDefaultValues() {
        foreach(var value in defaultValues.values) {
            var account = DefaultValues.ToAccount(value);
            dataCreator.CreateAccountAsync(account).GetAwaiter().GetResult();
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
