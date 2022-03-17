using PswManagerDatabase.DataAccess.Interfaces;

namespace PswManagerDatabase.DataAccess {
    public interface IDataConnection : IDataCreator, IDataReader, IDataEditor, IDataDeleter {


    }
}
