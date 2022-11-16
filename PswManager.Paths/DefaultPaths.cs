namespace PswManager.Paths;

/// <summary>
/// Provides a collection of static paths.
/// </summary>
public static class DefaultPaths {

    /// <summary>
    /// The directory that contains the exe this application is running from.
    /// </summary>
    public static string WorkingDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// The directory that contains all data related to this application.
    /// </summary>
    public static string DataDirectory { get; } = Path.Combine(WorkingDirectory, "Data");

    /// <summary>
    /// Acts as a temporary directory backup to change the master password.
    /// </summary>
    public static string BufferDataDirectory { get; } = Path.Combine(WorkingDirectory, "Buffer");

    /// <summary>
    /// The directory that contains ALL databases.
    /// </summary>
    public static string DatabaseDirectory { get; } = Path.Combine(DataDirectory, "Saves");

    /// <summary>
    /// The database directory used for the json database.
    /// </summary>
    public static string JsonDatabaseDirectory { get; } = Path.Combine(DatabaseDirectory, "Json");

    /// <summary>
    /// The database directory for saving with .txt files.
    /// </summary>
    public static string TextDatabaseDirectory { get; } = Path.Combine(DatabaseDirectory, "Text");

    /// <summary>
    /// The database directory for saving with a SQLite database.
    /// </summary>
    public static string SQLDatabaseDirectory { get; } = Path.Combine(DatabaseDirectory, "SQLite");

    /// <summary>
    /// The database file for saving with a SQLite database.
    /// </summary>
    public static string SQLDatabaseFile { get; } = Path.Combine(SQLDatabaseDirectory, "PswManager.db");

    /// <summary>
    /// The token file.
    /// </summary>
    public static string TokenFile { get; } = Path.Combine(DataDirectory, "Token.txt");

    /// <summary>
    /// The directory that contains all logs.
    /// </summary>
    public static string LogsDirectory { get; } = Path.Combine(WorkingDirectory, "Logs");

    /// <summary>
    /// The settings file.
    /// </summary>
    public static string SettingsFile { get; } = Path.Combine(DataDirectory, "Config.txt");

}