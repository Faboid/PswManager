using PswManagerDatabase.DataAccess;
using PswManagerDatabase.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase {
    public class DataFactory : IDataFactory {

        private readonly IDataConnection dataConnection;

        public DataFactory() {
            dataConnection = new DataConnection();
        }

        public IDataConnection GetDataConnection() => dataConnection;

        public IPathsEditor GetPathsEditor() => dataConnection;

        public IDataCreator GetDataCreator() => dataConnection;

        public IDataDeleter GetDataDeleter() => dataConnection;

        public IDataEditor GetDataEditor() => dataConnection;

        public IDataReader GetDataReader() => dataConnection;
    }
}
