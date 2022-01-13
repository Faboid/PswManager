using PswManagerDatabase.DataAccess;
using PswManagerDatabase.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase {

    public enum DatabaseType {
        TextFile
    }

    public class DataFactory : IDataFactory {

        private readonly IDataConnection dataConnection;

        public DataFactory(DatabaseType dbType) {

            dataConnection = dbType switch {
                DatabaseType.TextFile => new TextFileConnection(),
                _ => throw new ArgumentException("The given DatabaseType enum isn't supported.", nameof(dbType))
            };
        }

        public IDataConnection GetDataConnection() => dataConnection;

        public IPathsEditor GetPathsEditor() => dataConnection;

        public IDataCreator GetDataCreator() => dataConnection;

        public IDataDeleter GetDataDeleter() => dataConnection;

        public IDataEditor GetDataEditor() => dataConnection;

        public IDataReader GetDataReader() => dataConnection;
    }
}
