using PswManagerDatabase.DataAccess;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.DataAccess.MemoryDatabase;
using PswManagerDatabase.DataAccess.SQLDatabase;
using PswManagerDatabase.DataAccess.TextDatabase;
using PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("PswManagerTests")]
namespace PswManagerDatabase {

    public enum DatabaseType {
        TextFile,
        Sql,
        InMemory,
    }

    public class DataFactory : IDataFactory {

        private readonly IDataConnection dataConnection;

        public DataFactory(DatabaseType dbType) {

            dataConnection = dbType switch {
                DatabaseType.TextFile => new TextFileConnection(),
                DatabaseType.Sql => new SQLConnection(),
                DatabaseType.InMemory => new MemoryConnection(),
                _ => throw new ArgumentException("The given DatabaseType enum isn't supported.", nameof(dbType))
            };
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
                _ => throw new ArgumentException("The given DatabaseType enum isn't supported.", nameof(dbType))
            };

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            dataConnection = (IDataConnection)Activator.CreateInstance(dbClassType, flags, null, arguments, null);
        }

        public IDataConnection GetDataConnection() => dataConnection;

        public IDataCreator GetDataCreator() => dataConnection;

        public IDataDeleter GetDataDeleter() => dataConnection;

        public IDataEditor GetDataEditor() => dataConnection;

        public IDataReader GetDataReader() => dataConnection;

        public IDataHelper GetDataHelper() => dataConnection;
    }
}
