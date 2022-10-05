namespace PswManager.Paths;

/// <summary>
/// Provides a collection of paths and ensures that their directories exist.
/// </summary>
public interface IPathsBuilder {

    /// <summary>
    /// Gets the directory that contains the exe this application is running from.
    /// </summary>
    /// <returns></returns>
    string GetWorkingDirectory();

    /// <summary>
    /// Gets the directory that contains all data related to this application.
    /// </summary>
    /// <returns></returns>
    string GetDataDirectory();

    /// <summary>
    /// Gets the directory that contains ALL databases.
    /// </summary>
    string GetDatabaseDirectory();

    /// <summary>
    /// Gets the database directory used for the json database.
    /// </summary>
    string GetJsonDatabaseDirectory();
    
    /// <summary>
    /// Gets the database directory for saving with .txt files.
    /// </summary>
    string GetTextDatabaseDirectory();
    
    /// <summary>
    /// Gets the database directory for saving with a SQLite database.
    /// </summary>
    string GetSQLDatabaseDirectory();

    /// <summary>
    /// Gets the database file for saving with a SQLite database.
    /// </summary>
    string GetSQLDatabaseFile();

    /// <summary>
    /// Gets the token file.
    /// </summary>
    string GetTokenPath();

    /// <summary>
    /// Gets the directory that contains all logs.
    /// </summary>
    string GetLogsDirectory();

}
