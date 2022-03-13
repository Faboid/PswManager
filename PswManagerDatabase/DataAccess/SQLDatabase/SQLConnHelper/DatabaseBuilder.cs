using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper {
    internal class DatabaseBuilder {

        public DatabaseBuilder(string db_name) {
            db_Name = db_name;
            SetUpDatabase();
        }

        private static readonly string WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string DataDirectoryPath = Path.Combine(WorkingDirectory, "Data");
        private readonly string db_Name;

        public SQLiteConnection GetConnection() {
            return new($"Data Source={DataDirectoryPath}\\{db_Name}.db; Version=3;");
        }

        private void SetUpDatabase() {
            if(!Directory.Exists(DataDirectoryPath)) { 
                Directory.CreateDirectory(DataDirectoryPath);
            }

            using var cnn = GetConnection();
            cnn.Open();
            CreateLiteAccountsTable(cnn);
            cnn.Close();
        }

        private static void CreateLiteAccountsTable(SQLiteConnection cnn) {

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
