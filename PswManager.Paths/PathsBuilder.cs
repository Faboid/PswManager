using System.IO.Abstractions;

namespace PswManager.Paths;

public interface IPathsBuilder {
    string GetDatabaseDirectory();
    string GetDataDirectory();
    string GetWorkingDirectory();
}

public class PathsBuilder : IPathsBuilder {

    public PathsBuilder(IDirectoryInfoFactory directoryInfoFactory) {
        directoryInfoFactory.FromDirectoryName(GetDatabaseDirectory()).Create();
    }

    public string GetWorkingDirectory() => DefaultPaths.WorkingDirectory;
    public string GetDataDirectory() => DefaultPaths.DataDirectory;
    public string GetDatabaseDirectory() => DefaultPaths.DatabaseDirectory;

}

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
    /// The directory that contains ALL databases.
    /// </summary>
    public static string DatabaseDirectory { get; } = Path.Combine(DataDirectory, "Saves");

    /// <summary>
    /// The token file.
    /// </summary>
    public static string TokenFile { get; } = Path.Combine(DataDirectory, "Token.txt");

    /// <summary>
    /// The directory that contains all logs.
    /// </summary>
    public static string LogsDirectory { get; } = Path.Combine(DataDirectory, "Logs");

    /// <summary>
    /// The settings file.
    /// </summary>
    public static string SettingsFile { get; } = Path.Combine(DataDirectory, "Config.txt");

}