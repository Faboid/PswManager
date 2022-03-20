using PswManagerDatabase.Config;
using System.IO;

namespace PswManagerTests.Database.TextFileConnectionTests.Helpers {
    internal class CustomPaths : IPaths {

        public CustomPaths(string folderPath) {
            WorkingDirectory = folderPath;
        }

        public string WorkingDirectory { get; }

        public string PasswordsFilePath => Path.Combine(WorkingDirectory, "Passwords.txt");

        public string AccountsFilePath => Path.Combine(WorkingDirectory, "Accounts.txt");

        public string EmailsFilePath => Path.Combine(WorkingDirectory, "Emails.txt");

        public string TokenFilePath => Path.Combine(WorkingDirectory, "Token.txt");

        public void MoveMain(string path) {
            throw new System.NotImplementedException();
        }

        public void SetMain(string path) {
            throw new System.NotImplementedException();
        }
    }
}
