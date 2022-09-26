using PswManager.Database.DataAccess.Interfaces;

namespace PswManager.Database;
public interface IDataConnection : IDataCreator, IDataReader, IDataEditor, IDataDeleter {}
