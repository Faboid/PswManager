using PswManager.Database.Models;
using System.Data.SQLite;

namespace PswManager.Database.DataAccess.SQLDatabase.SQLConnHelper;
internal class QueriesBuilder {

    public QueriesBuilder(SQLiteConnection connection) {
        this.connection = connection;
    }

    private readonly SQLiteConnection connection;
    private const string accountsTable = "Accounts";

    public SQLiteCommand CreateAccountQuery(IReadOnlyAccountModel model) {
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

    public SQLiteCommand DeleteAccountQuery(string name) {
        string query = $"delete from {accountsTable} where Name = @Name";
        var cmd = new SQLiteCommand(query, connection);
        cmd.Parameters.Add(new SQLiteParameter("@Name", name));

        return cmd;
    }

    public SQLiteCommand UpdateAccountQuery(string name, AccountModel newModel) {
        string query = $"update {accountsTable} set " +
            $"Name = case when @NewName is not null then @NewName else Name end, " +
            $"Password = case when @NewPassword is not null then @NewPassword else Password end, " +
            $"Email = case when @NewEmail is not null then @NewEmail else Email end " +
            $"where Name = @Name";

        var cmd = new SQLiteCommand(query, connection);
        cmd.Parameters.Add(new SQLiteParameter("@Name", name));
        cmd.Parameters.Add(new SQLiteParameter("@NewName", NullIfEmpty(newModel.Name)));
        cmd.Parameters.Add(new SQLiteParameter("@NewPassword", NullIfEmpty(newModel.Password)));
        cmd.Parameters.Add(new SQLiteParameter("@NewEmail", NullIfEmpty(newModel.Email)));

        return cmd;
    }

    private static string NullIfEmpty(string value) {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

}
