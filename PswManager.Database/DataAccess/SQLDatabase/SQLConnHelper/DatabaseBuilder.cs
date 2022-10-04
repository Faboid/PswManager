using PswManager.Paths;
using System.Data.SQLite;
using System.IO;

namespace PswManager.Database.DataAccess.SQLDatabase.SQLConnHelper;
internal class DatabaseBuilder {

    public DatabaseBuilder(IPathsBuilder pathsBuilder) {
        _dataDirectoryPath = pathsBuilder.GetSQLDatabaseDirectory();
        _dbPath = pathsBuilder.GetSQLDatabaseFile();
        SetUpDatabase();
    }

    private readonly string _dataDirectoryPath;
    private readonly string _dbPath;

    public SQLiteConnection GetConnection() {
        return new($"Data Source={_dbPath}; Version=3;");
    }

    private void SetUpDatabase() {
        if(!Directory.Exists(_dataDirectoryPath)) {
            Directory.CreateDirectory(_dataDirectoryPath);
        }

        using var cnn = GetConnection();
        cnn.Open();
        CreateAccountsTable(cnn);
        cnn.Close();
    }

    private static void CreateAccountsTable(SQLiteConnection cnn) {

        string query = $"create table if not exists Accounts" +
            $"(" +
            $"Name nvarchar(50) PRIMARY KEY NOT NULL," +
            $"Password nvarchar(100) NOT NULL," +
            $"Email nvarchar(100) NOT NULL" +
            $") WITHOUT ROWID";

        using SQLiteCommand cmd = new(query, cnn);
        cmd.ExecuteNonQuery();
    }

}
