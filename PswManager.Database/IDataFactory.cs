using PswManager.Database.DataAccess.Interfaces;

namespace PswManager.Database;
public interface IDataFactory {

    IDataConnection GetDataConnection();
    IDataCreator GetDataCreator();
    IDataReader GetDataReader();
    IDataEditor GetDataEditor();
    IDataDeleter GetDataDeleter();
    IDataHelper GetDataHelper();

}
