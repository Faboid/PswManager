using PswManagerDatabase.Config;
using PswManagerDatabase.DataAccess;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.DataAccess.SQLDatabase;
using PswManagerDatabase.DataAccess.TextDatabase;
using System;

namespace PswManagerDatabase {

    public enum DatabaseType {
        TextFile,
        Sql
    }

    public class DataFactory : IDataFactory {

        private readonly IDataConnection dataConnection;

        public DataFactory(DatabaseType dbType) {

            dataConnection = dbType switch {
                DatabaseType.TextFile => new TextFileConnection(new Paths()),
                DatabaseType.Sql => new SQLConnection("PswManagerDB"),
                _ => throw new ArgumentException("The given DatabaseType enum isn't supported.", nameof(dbType))
            };
        }

        public DataFactory(IPaths customPaths) {
            dataConnection = new TextFileConnection(customPaths);
        }

        public DataFactory(string customDBName) {
            dataConnection = new SQLConnection(customDBName);
        }

        public IDataConnection GetDataConnection() => dataConnection;

        public IDataCreator GetDataCreator() => dataConnection;

        public IDataDeleter GetDataDeleter() => dataConnection;

        public IDataEditor GetDataEditor() => dataConnection;

        public IDataReader GetDataReader() => dataConnection;

        public IDataHelper GetDataHelper() => dataConnection;
    }
}
