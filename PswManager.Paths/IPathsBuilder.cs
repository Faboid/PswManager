namespace PswManager.Paths;

public interface IPathsBuilder {
    string GetDatabaseDirectory();
    string GetDataDirectory();
    string GetJsonDatabaseDirectory();
    string GetLogsDirectory();
    string GetSQLDatabaseDirectory();
    string GetSQLDatabaseFile();
    string GetTextDatabaseDirectory();
    string GetTokenPath();
    string GetWorkingDirectory();
}
