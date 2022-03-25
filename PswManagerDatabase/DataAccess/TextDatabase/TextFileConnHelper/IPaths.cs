namespace PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper {
    public interface IPaths {

        static string WorkingDirectory { get; }

        string PasswordsFilePath { get; }

        string AccountsFilePath { get; }

        string EmailsFilePath { get; }

        string TokenFilePath { get; }

    }
}
