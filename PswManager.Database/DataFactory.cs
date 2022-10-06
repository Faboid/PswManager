using PswManager.Database.DataAccess;
using PswManager.Database.DataAccess.JsonDatabase;
using PswManager.Database.DataAccess.MemoryDatabase;
using PswManager.Database.DataAccess.SQLDatabase;
using PswManager.Database.DataAccess.TextDatabase;
using PswManager.Database.Interfaces;
using PswManager.Database.Wrappers;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PswManager.Database.Tests")]
namespace PswManager.Database;
public enum DatabaseType {
    TextFile,
    Sql,
    InMemory,
    Json
}

public class DataFactory : IDataFactory {

    private readonly IDataConnection dataConnection;

    /// <summary>
    /// Instances <see cref="DataFactory"/> that contains an <see cref="IDataConnection"/> of the given <paramref name="dbType"/>.
    /// </summary>
    /// <param name="dbType"></param>
    /// <exception cref="ArgumentException"></exception>
    public DataFactory(DatabaseType dbType) {

        IDBConnection dbConnection = dbType switch {
            DatabaseType.TextFile => new TextFileConnection(),
            DatabaseType.Sql => new SQLConnection(),
            DatabaseType.InMemory => new MemoryConnection(),
            DatabaseType.Json => new JsonConnection(),
            _ => throw new ArgumentException("The given DatabaseType enum isn't supported.", nameof(dbType))
        };

        dataConnection = new WrappersBuilder(dbConnection).BuildWrappers();
    }

    /// <summary>
    /// This allows creating a database with custom parameters. Note: it's to be used only for testing.
    /// </summary>
    /// <param name="dbType"></param>
    /// <param name="arguments"></param>
    /// <exception cref="ArgumentException"></exception>
    internal DataFactory(DatabaseType dbType, params object[] arguments) {
        var dbClassType = dbType switch {
            DatabaseType.TextFile => typeof(TextFileConnection),
            DatabaseType.Sql => typeof(SQLConnection),
            DatabaseType.InMemory => typeof(MemoryConnection),
            DatabaseType.Json => typeof(JsonConnection),
            _ => throw new ArgumentException("The given DatabaseType enum isn't supported.", nameof(dbType))
        };

        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var dbConnection = (IDBConnection)Activator.CreateInstance(dbClassType, flags, null, arguments, null);
        dataConnection = new WrappersBuilder(dbConnection).BuildWrappers();
    }

    public IDataConnection GetDataConnection() => dataConnection;

    public IDataCreator GetDataCreator() => dataConnection;

    public IDataDeleter GetDataDeleter() => dataConnection;

    public IDataEditor GetDataEditor() => dataConnection;

    public IDataReader GetDataReader() => dataConnection;

    public IDataHelper GetDataHelper() => dataConnection;
}
