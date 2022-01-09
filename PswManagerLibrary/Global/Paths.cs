using PswManagerLibrary.Generic;
using System;
using System.IO;
using System.Linq;
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

        private const string passwordsFileName = "Passwords.txt";
        private const string accountsFileName = "Accounts.txt";
        private const string emailsFileName = "Emails.txt";
        private const string tokenFileName = "Token.txt";

        public string PasswordsFilePath => Path.Combine(GetMain(), passwordsFileName);

        public string AccountsFilePath => Path.Combine(GetMain(), accountsFileName);

        public string EmailsFilePath => Path.Combine(GetMain(), emailsFileName);

        public string TokenFilePath => Path.Combine(GetMain(), tokenFileName);

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
            if(path == GetMain()) {
                return;
            }

            if(Directory.Exists(path) == false) {
                throw new ArgumentException("The given path doesn't point to an existing directory.", nameof(path));
            }

            string[] pathNames = new string[] { passwordsFileName, accountsFileName, emailsFileName, tokenFileName };
            string currentMain = GetMain();
            
            //checks if there's any existing file among the future new paths
            if(pathNames.Any(x => File.Exists(Path.Combine(path, x)))) {
                throw new ArgumentException("The given path is already occupied by same-named files.", nameof(path));
            }

            try {

                //moves the files to the new folder
                pathNames.ForEach(x => File.Move(Path.Combine(currentMain, x), Path.Combine(path, x)));

                //sets main to point to the new directory
                SetMain(path);

            } catch (IOException) {

                //if the operations fail, rollback to the previous version
                SetMain(currentMain);
                
                pathNames.ForEach(x => { 
                    if(File.Exists(Path.Combine(path, x))) 
                        File.Move(Path.Combine(path, x), Path.Combine(currentMain, x)); 
                });

                throw;
            }

        }

        private string GetMain() {
            return File.ReadAllText(ConfigFilePath);
        }

        private void SetDefaultMain() {
            SetMain(WorkingDirectory);
        }

    }
}
