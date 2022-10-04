namespace PswManager.Paths;

public interface IPathsBuilder {
    string GetDatabaseDirectory();
    string GetDataDirectory();
    string GetJsonDatabaseDirectory();
    string GetSQLDatabaseDirectory();
    string GetSQLDatabaseFile();
    string GetTextDatabaseDirectory();
    string GetWorkingDirectory();
}
