using System.IO.Abstractions;

namespace PswManager.Paths;

public class PathsBuilder : IPathsBuilder {

    public PathsBuilder() : this(new FileSystem().DirectoryInfo) {}
    public PathsBuilder(IDirectoryInfoFactory directoryInfoFactory) {
        directoryInfoFactory.FromDirectoryName(GetDatabaseDirectory()).Create();
        directoryInfoFactory.FromDirectoryName(GetJsonDatabaseDirectory()).Create();
        directoryInfoFactory.FromDirectoryName(GetTextDatabaseDirectory()).Create();
        directoryInfoFactory.FromDirectoryName(GetSQLDatabaseDirectory()).Create();
    }

    public string GetWorkingDirectory() => DefaultPaths.WorkingDirectory;
    public string GetDataDirectory() => DefaultPaths.DataDirectory;
    public string GetDatabaseDirectory() => DefaultPaths.DatabaseDirectory;
    public string GetJsonDatabaseDirectory() => DefaultPaths.JsonDatabaseDirectory;
    public string GetTextDatabaseDirectory() => DefaultPaths.TextDatabaseDirectory;
    public string GetSQLDatabaseDirectory() => DefaultPaths.SQLDatabaseDirectory;
    public string GetSQLDatabaseFile() => DefaultPaths.SQLDatabaseFile;
}