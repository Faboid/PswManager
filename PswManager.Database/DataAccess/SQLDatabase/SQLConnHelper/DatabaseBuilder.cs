using PswManager.Utils;
using System.Data.SQLite;
using System.IO;

namespace PswManager.Database.DataAccess.SQLDatabase.SQLConnHelper {
    internal class DatabaseBuilder {

        public DatabaseBuilder(string db_name) {
            dataDirectoryPath = Path.Combine(WorkingDirectory, "Data");
            dbPath = Path.Combine(dataDirectoryPath, $"{db_name}.db");
            SetUpDatabase();
        }

        private static readonly string WorkingDirectory = PathsBuilder.GetWorkingDirectory;
        private readonly string dataDirectoryPath;
        private readonly string dbPath;

        public SQLiteConnection GetConnection() {
            return new($"Data Source={dbPath}; Version=3;");
        }

        private void SetUpDatabase() {
            if(!Directory.Exists(dataDirectoryPath)) { 
                Directory.CreateDirectory(dataDirectoryPath);
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
}
