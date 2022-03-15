using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper {
    internal class QueriesBuilder {

        public QueriesBuilder(SQLiteConnection connection) {
            this.connection = connection;
        }

        private readonly SQLiteConnection connection;
        private const string accountsTable = "Accounts";

        public SQLiteCommand CreateAccountQuery(AccountModel model) {
            string query = $"insert into {accountsTable} " +
                $"values(@Name, @Password, @Email)";
            var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.Add(new SQLiteParameter("@Name", model.Name));
            cmd.Parameters.Add(new SQLiteParameter("@Password", model.Password));
            cmd.Parameters.Add(new SQLiteParameter("@Email", model.Email));

            return cmd;
        }

        public SQLiteCommand GetAccountQuery(string name) {
            string query = $"select * from {accountsTable} where Name = @Name";
            var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.Add(new SQLiteParameter("@Name", name));

            return cmd;
        }

        public SQLiteCommand GetAllAccountsQuery() {
            string query = $"select * from {accountsTable}";
            var cmd = new SQLiteCommand(query, connection);

            return cmd;
        }

    }
}
