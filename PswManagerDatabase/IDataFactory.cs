using PswManagerDatabase.DataAccess;
using PswManagerDatabase.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase {
    public interface IDataFactory {

        IDataConnection GetDataConnection();
        IPathsEditor GetPathsEditor();
        IDataCreator GetDataCreator();
        IDataReader GetDataReader();
        IDataEditor GetDataEditor();
        IDataDeleter GetDataDeleter();

    }
}
