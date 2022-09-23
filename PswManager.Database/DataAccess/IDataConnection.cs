using PswManager.Database.DataAccess.Interfaces;

namespace PswManager.Database.DataAccess; 
public interface IDataConnection : IDataCreator, IDataReader, IDataEditor, IDataDeleter {


}
