using PswManagerLibrary.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerTests {
    public class TestsPaths : IPaths {

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

    }
}
