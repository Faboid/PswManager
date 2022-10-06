using System.IO.Abstractions;

namespace PswManager.Paths;

/// <summary>
/// <inheritdoc cref="IPathsBuilder"/>
/// </summary>
public class PathsBuilder : IPathsBuilder {

    /// <summary>
    /// Initializes <see cref="PathsBuilder"/> and creates all non-existing directories among the paths.
    /// </summary>
    public PathsBuilder() : this(new FileSystem().DirectoryInfo) { }

    /// <summary>
    /// Initializes <see cref="PathsBuilder"/> and creates all non-existing directories 
    /// with <see cref="IDirectoryInfoFactory.FromDirectoryName(string)"/>'s <see cref="IDirectoryInfo.Create"/>.
    /// </summary>
    /// <param name="directoryInfoFactory"></param>
    public PathsBuilder(IDirectoryInfoFactory directoryInfoFactory) {
        directoryInfoFactory.FromDirectoryName(GetDataDirectory()).Create();
        directoryInfoFactory.FromDirectoryName(GetLogsDirectory()).Create();
        directoryInfoFactory.FromDirectoryName(GetDatabaseDirectory()).Create();
        directoryInfoFactory.FromDirectoryName(GetJsonDatabaseDirectory()).Create();
        directoryInfoFactory.FromDirectoryName(GetTextDatabaseDirectory()).Create();
        directoryInfoFactory.FromDirectoryName(GetSQLDatabaseDirectory()).Create();
    }

    public string GetTokenPath() => DefaultPaths.TokenFile;
    public string GetWorkingDirectory() => DefaultPaths.WorkingDirectory;
    public string GetDataDirectory() => DefaultPaths.DataDirectory;
    public string GetLogsDirectory() => DefaultPaths.LogsDirectory;
    public string GetDatabaseDirectory() => DefaultPaths.DatabaseDirectory;
    public string GetJsonDatabaseDirectory() => DefaultPaths.JsonDatabaseDirectory;
    public string GetTextDatabaseDirectory() => DefaultPaths.TextDatabaseDirectory;
    public string GetSQLDatabaseDirectory() => DefaultPaths.SQLDatabaseDirectory;
    public string GetSQLDatabaseFile() => DefaultPaths.SQLDatabaseFile;
}