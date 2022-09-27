using PswManager.Database.Interfaces;

namespace PswManager.Database;
public interface IDataConnection : IDataCreator, IDataReader, IDataEditor, IDataDeleter {}
