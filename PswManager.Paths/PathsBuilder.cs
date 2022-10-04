using System.IO.Abstractions;

namespace PswManager.Paths;

public class PathsBuilder : IPathsBuilder {

    public PathsBuilder(IDirectoryInfoFactory directoryInfoFactory) {
        directoryInfoFactory.FromDirectoryName(GetDatabaseDirectory()).Create();
    }

    public string GetWorkingDirectory() => DefaultPaths.WorkingDirectory;
    public string GetDataDirectory() => DefaultPaths.DataDirectory;
    public string GetDatabaseDirectory() => DefaultPaths.DatabaseDirectory;

}