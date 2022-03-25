using PswManagerHelperMethods;
using System.IO;

namespace PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper {
    internal class Paths : IPaths {

        public Paths() {
            if(!Directory.Exists(DataDirectory)) {
                Directory.CreateDirectory(DataDirectory);
            }
        }

        public static string DataDirectory { get; } = Path.Combine(PathsBuilder.GetWorkingDirectory, "Data");

        private const string passwordsFileName = "Passwords.txt";
        private const string accountsFileName = "Accounts.txt";
        private const string emailsFileName = "Emails.txt";
        private const string tokenFileName = "Token.txt";

        public string PasswordsFilePath => Path.Combine(DataDirectory, passwordsFileName);

        public string AccountsFilePath => Path.Combine(DataDirectory, accountsFileName);

        public string EmailsFilePath => Path.Combine(DataDirectory, emailsFileName);

        public string TokenFilePath => Path.Combine(DataDirectory, tokenFileName);

    }
}
