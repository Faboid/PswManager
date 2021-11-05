using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Global {

    /// <summary>
    /// Stores global paths.
    /// </summary>
    public class Paths : IPaths {

        public Paths() { }


        public static string WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        //todo - temporary stored location. Will change to storing it in a config or a txt file
        public string PasswordsFilePath => $"{WorkingDirectory}\\Passwords.txt";

        public string AccountsFilePath => $"{WorkingDirectory}\\Accounts.txt";

        public string EmailsFilePath => $"{WorkingDirectory}\\Emails.txt";

    }
}
