using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper {
    internal class DatabaseBuilder {

        public DatabaseBuilder(string db_name) {
            db_Name = db_name;
        }

        private static readonly string WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private const string masterConnection = $"server={serverName}; Integrated security=SSPI; database=master";
        private const string serverName = "localhost";
        private readonly string db_Name;

        private string GetConnectionString() {

            //todo - if server=localhost doesn't work, try .\SQLEXPRESS or (local)
            SqlConnectionStringBuilder builder = new(string.Format("server={0}; Integrated security=SSPI; database={1}", serverName, db_Name));
            builder.ApplicationName = "PswManager";
            builder.ApplicationIntent = ApplicationIntent.ReadWrite;

            return builder.ToString();
        }

        //todo - there's no point in going through the whole database checking to get the connection string,
        //so it's best to move it to the constructor or something similar
        public SqlConnection GetConnection() {

            //try to get database connection
            if(CheckDatabaseExistence()) {
                return new(GetConnectionString());
            }

            //if fail, create new database
            CreateDatabase();

            //check once more
            if(CheckDatabaseExistence()) {
                return new(GetConnectionString());
            } else {
                //todo - handle this in some way
                throw new Exception();
            }
        }

        private bool CheckDatabaseExistence() {
            //credits for this method to:
            //https://stackoverflow.com/questions/2232227/check-if-database-exists-before-creating/52817252#52817252
            var conn = new SqlConnection(masterConnection);

            try {
                using SqlCommand cmd = new("SELECT db_id(@DBName)", conn);
                cmd.Parameters.Add(new SqlParameter("@DBName", db_Name));

                conn.Open();

                //if the result isn't null, a matching database has been found
                return cmd.ExecuteScalar() != DBNull.Value;

            } finally {
                if(conn.State == System.Data.ConnectionState.Open) {
                    conn.Close();
                }
            }
        }

        private void CreateDatabase() {
            //credit for this method to:
            //https://docs.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/create-sql-server-database-programmatically

            using SqlConnection cnn = new(masterConnection);
            string fileName = $"{ WorkingDirectory }\\{db_Name}";

            string query = $"CREATE DATABASE {db_Name} ON PRIMARY" +
                $"(NAME = {db_Name}_Data," +
                $"FILENAME = {fileName}.mdf," +
                $"SIZE = 2MB, MAXSIZE = 100MB, FILEGROWTH = 10%)" +
                $"LOG ON (NAME = {db_Name}_Log," +
                $"FILENAME = {fileName}.ldf," +
                $"SIZE = 1MB, MAXSIZE = 20MB, FILEGROWTH = 10%)";

            using SqlCommand cmd = new(query, cnn);
            try {
                cnn.Open();
                cmd.ExecuteNonQuery();
                CreateAccountsTable(cnn);
            } finally {
                if(cnn.State == System.Data.ConnectionState.Open) {
                    cnn.Close();
                }
            }

        }

        private static void CreateAccountsTable(SqlConnection cnn) {

            string query = "CREATE TABLE Accounts(" +
                "[Name] nvarchar(50) NOT NULL PRIMARY KEY," +
                "[Password] nvarchar(100) NOT NULL," +
                "[Email] nvarchar(50) NOT NULL)";

            using SqlCommand cmd = new(query, cnn);
            cmd.ExecuteNonQuery();
        }


    }
}
