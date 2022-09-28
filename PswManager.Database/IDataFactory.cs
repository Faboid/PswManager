using PswManager.Database.Interfaces;

namespace PswManager.Database;
public interface IDataFactory {

    IDataConnection GetDataConnection();
    IDataCreator GetDataCreator();
    IDataReader GetDataReader();
    IDataEditor GetDataEditor();
    IDataDeleter GetDataDeleter();
    IDataHelper GetDataHelper();

}
