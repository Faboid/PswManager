using PswManagerLibrary.Global;
using System.IO;
using System.Reflection;

namespace PswManagerTests.TestsHelpers {
    public class TestsPaths : IPaths, PswManagerDatabase.Config.IPaths {

        public TestsPaths() { }

        public string WorkingDirectory = GetNonExistentFolderPath();

        public string PasswordsFilePath => $"{WorkingDirectory}\\Passwords.txt";

        public string AccountsFilePath => $"{WorkingDirectory}\\Accounts.txt";

        public string EmailsFilePath => $"{WorkingDirectory}\\Emails.txt";

        public string TokenFilePath => $"{WorkingDirectory}\\Token.txt";

        public static string GetNonExistentFolderPath() {
            string output;

            string basepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            int curr = 0;
            do {
                curr++;
                output = Path.Combine(basepath, curr.ToString());
            } while(Directory.Exists(output));

            return output;
        }

        public void SetMain(string path) {
            throw new System.NotImplementedException();
        }

        public void MoveMain(string path) {
            throw new System.NotImplementedException();
        }
    }
}
