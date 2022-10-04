namespace PswManager.Paths;

public interface IPathsBuilder {
    string GetDatabaseDirectory();
    string GetDataDirectory();
    string GetWorkingDirectory();
}
