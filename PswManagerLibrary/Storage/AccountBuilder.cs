using PswManagerLibrary.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Storage {

    /// <summary>
    /// A class whose purpose is to connect the three storage files: accountsfile, passwordsfile, and emailsfile.
    /// </summary>
    public class AccountBuilder {

        IPaths paths;

        public AccountBuilder(IPaths paths) {
            this.paths = paths;
        }

        /// <summary>
        /// Returns the position of the name. If it doesn't find any, returns null.
        /// </summary>
        public int? Search(string name) {
            int position = 0;

            using(StreamReader reader = new StreamReader(paths.AccountsFilePath)) {
                string current;
                while((current = reader.ReadLine()) != name) {
                    position++;
                    
                    if(current is null) {
                        return null;
                    }
                }
            }

            return position;
        }

        public string GetOne(int position) {

            string name = File.ReadAllLines(paths.AccountsFilePath).Skip(position).Take(1).First();
            string password = File.ReadAllLines(paths.PasswordsFilePath).Skip(position).Take(1).First();
            string email = File.ReadAllLines(paths.EmailsFilePath).Skip(position).Take(1).First();

            return StringToAccount(name, password, email);
        }

        public static string StringToAccount(string name, string email, string password) => $"{name}:{Environment.NewLine}{email}{Environment.NewLine}{password}";

    }
}
