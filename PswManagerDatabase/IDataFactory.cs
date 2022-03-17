using PswManagerDatabase.DataAccess;
using PswManagerDatabase.DataAccess.Interfaces;

namespace PswManagerDatabase {
    public interface IDataFactory {

        IDataConnection GetDataConnection();
        IDataCreator GetDataCreator();
        IDataReader GetDataReader();
        IDataEditor GetDataEditor();
        IDataDeleter GetDataDeleter();
        IDataHelper GetDataHelper();

    }
}
