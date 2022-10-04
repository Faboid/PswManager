using Moq;
using PswManager.Paths;
using System.IO.Abstractions.TestingHelpers;

namespace PswManager.Database.Tests.Mocks;

public class MockPathsBuilder : IPathsBuilder {

    public MockPathsBuilder() {
        _guid = Guid.NewGuid().ToString();
    }

    public MockPathsBuilder(string name) {
        _guid = $"{Guid.NewGuid()}{name}";
    }

    private readonly string _guid;
    private readonly PathsBuilder _pathsBuilder = new(new MockFileSystem().DirectoryInfo);

    public string GetDataDirectory() => Path.Combine(GetWorkingDirectory(), "TestData");
    public string GetJsonDatabaseDirectory() => Path.Combine(GetDatabaseDirectory(), "Json", _guid);
    public string GetSQLDatabaseDirectory() => Path.Combine(GetDatabaseDirectory(), "SQLDB", _guid);
    public string GetSQLDatabaseFile() => Path.Combine(GetSQLDatabaseDirectory(), "DB.db");
    public string GetTextDatabaseDirectory() => Path.Combine(GetDatabaseDirectory(), "TextDB", _guid);
    public string GetDatabaseDirectory() => Path.Combine(_pathsBuilder.GetDataDirectory(), "Saves", _guid);
    public string GetWorkingDirectory() => _pathsBuilder.GetWorkingDirectory();
    public string GetLogsDirectory() => throw new NotSupportedException();
    public string GetTokenPath() => throw new NotSupportedException();
}