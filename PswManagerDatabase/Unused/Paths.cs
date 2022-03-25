using PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper;
using PswManagerHelperMethods;
using System;
using System.IO;
using System.Linq;

namespace PswManagerDatabase.Unused {

    /// <summary>
    /// Stores global paths.
    /// </summary>
    public class Paths : IPaths {

        public Paths() {
            if(!File.Exists(ConfigFilePath) || !Directory.Exists(GetMain())) {
                if(!Directory.Exists(DataDirectory))
                    Directory.CreateDirectory(DataDirectory);

                SetDefaultMain();
            }
        }

        public static string DataDirectory { get; } = Path.Combine(PathsBuilder.GetWorkingDirectory, "Data");
        private static string ConfigFilePath { get; } = Path.Combine(DataDirectory, "Config.txt");

        private const string passwordsFileName = "Passwords.txt";
        private const string accountsFileName = "Accounts.txt";
        private const string emailsFileName = "Emails.txt";
        private const string tokenFileName = "Token.txt";

        public string PasswordsFilePath => Path.Combine(GetMain(), passwordsFileName);

        public string AccountsFilePath => Path.Combine(GetMain(), accountsFileName);

        public string EmailsFilePath => Path.Combine(GetMain(), emailsFileName);

        public string TokenFilePath => Path.Combine(GetMain(), tokenFileName);

        //todo - remove all exceptions and return a ConnectionResult in their stead.

        /// <summary>
        /// Changes the path to the accounts WITHOUT dealing with the current saved data. Any folder and files at the previous path will remain. The new directory must be existent.
        /// </summary>
        /// <param name="path">The path pointing to the new directory.</param>
        public void SetMain(string path) {
            if(Directory.Exists(path) == false) {
                throw new ArgumentException("The given path doesn't point to an existing directory.", nameof(path));
            }

            File.WriteAllText(ConfigFilePath, path);
        }

        /// <summary>
        /// Moves the saved data to the new directory and sets it as the current database path. The new directory must be existent.
        /// </summary>
        /// <param name="path">The path pointing to the new directory.</param>
        public void MoveMain(string path) {
            if(path == GetMain()) {
                return;
            }

            if(Directory.Exists(path) == false) {
                throw new ArgumentException("The given path doesn't point to an existing directory.", nameof(path));
            }

            string[] pathNames = new string[] { passwordsFileName, accountsFileName, emailsFileName, tokenFileName };
            string currentMain = GetMain();
            string GetOldPath(string s) => Path.Combine(currentMain, s);
            string GetNewPath(string s) => Path.Combine(path, s);

            //checks if there's any existing file among the future new paths
            if(pathNames.Any(x => File.Exists(GetNewPath(x)))) {
                throw new ArgumentException("The given path is already occupied by same-named files.", nameof(path));
            }

            try {

                //moves the files to the new folder
                pathNames.ForEach(x => File.Move(GetOldPath(x), GetNewPath(x)));

                //sets main to point to the new directory
                SetMain(path);

            }
            catch(IOException) {

                //if the operations fail, rollback to the previous version
                SetMain(currentMain);

                pathNames.ForEach(x => {
                    try {
                        if(File.Exists(GetNewPath(x)))
                            File.Move(GetNewPath(x), GetOldPath(x));
                    }
                    catch { };
                });

                //todo - turn the MoveMain() operation's return method into a ConnectionResult, so that it can be solved without exceptions
                throw;
            }

        }

        private string GetMain() {
            return File.ReadAllText(ConfigFilePath);
        }

        private void SetDefaultMain() {
            SetMain(DataDirectory);
        }

    }
}
