using PswManager.Database.Interfaces;

namespace PswManager.Database;

/// <summary>
/// Provides methods to obtain an instance of <see cref="IDataConnection"/>.
/// </summary>
public interface IDataFactory {

    /// <summary>
    /// Returns a shared instance of <see cref="IDataConnection"/>.
    /// </summary>
    /// <returns></returns>
    IDataConnection GetDataConnection();

    /// <summary>
    /// Returns a shared instance of <see cref="IDataCreator"/>.
    /// </summary>
    /// <returns></returns>
    IDataCreator GetDataCreator();

    /// <summary>
    /// Returns a shared instance of <see cref="IDataReader"/>.
    /// </summary>
    /// <returns></returns>
    IDataReader GetDataReader();

    /// <summary>
    /// Returns a shared instance of <see cref="IDataEditor"/>.
    /// </summary>
    /// <returns></returns>
    IDataEditor GetDataEditor();

    /// <summary>
    /// Returns a shared instance of <see cref="IDataDeleter"/>.
    /// </summary>
    /// <returns></returns>
    IDataDeleter GetDataDeleter();

    /// <summary>
    /// Returns a shared instance of <see cref="IDataHelper"/>.
    /// </summary>
    /// <returns></returns>
    IDataHelper GetDataHelper();

}
