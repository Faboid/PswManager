using PswManagerDatabase.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper {
    internal class QueriesBuilder {

        private const string accountsTable = "Accounts";

        public static SqlCommand CreateAccountQuery(AccountModel model) {
            string query = $"insert into {accountsTable}" +
                $"values(@Name, @Password, @Email)";
            var cmd = new SqlCommand(query);
            cmd.Parameters.Add(new SqlParameter("@Name", model.Name));
            cmd.Parameters.Add(new SqlParameter("@Password", model.Password));
            cmd.Parameters.Add(new SqlParameter("@Email", model.Email));

            return cmd;
        }

        public static SqlCommand GetAccountQuery(string name) {
            string query = $"select * from {accountsTable} where Name = @Name";
            var cmd = new SqlCommand(query);
            cmd.Parameters.Add(new SqlParameter("@Name", name));

            return cmd;
        }

    }
}
