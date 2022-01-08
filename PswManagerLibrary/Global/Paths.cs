using System;
using System.IO;
using System.Reflection;

namespace PswManagerLibrary.Global {

    /// <summary>
    /// Stores global paths.
    /// </summary>
    public class Paths : IPaths {

        public Paths() { 
            if(!File.Exists(ConfigFilePath) || !Directory.Exists(GetMain())) {
                SetDefaultMain();
            }
        }


        public readonly static string WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly static string ConfigFilePath = Path.Combine(WorkingDirectory, "Config.txt");

        public string PasswordsFilePath => Path.Combine(GetMain(), "Passwords.txt");

        public string AccountsFilePath => Path.Combine(GetMain(), "Accounts.txt");

        public string EmailsFilePath => Path.Combine(GetMain(), "Emails.txt");

        public string TokenFilePath => Path.Combine(GetMain(), "Token.txt");

        /// <summary>
        /// Changes the path to the accounts WITHOUT dealing with the current saved data. Any folder and files at the previous path will remain.
        /// </summary>
        /// <param name="path"></param>
        public void SetMain(string path) {
            if(Directory.Exists(path) == false) {
                throw new ArgumentException("The given path doesn't point to an existing directory.", nameof(path));
            }

            File.WriteAllText(ConfigFilePath, path);
        }

        public void MoveMain(string path) {
            throw new NotImplementedException();
        }

        private string GetMain() {
            return File.ReadAllText(ConfigFilePath);
        }

        private void SetDefaultMain() {
            SetMain(WorkingDirectory);
        }

    }
}
