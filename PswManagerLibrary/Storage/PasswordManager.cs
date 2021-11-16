using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Exceptions;
using PswManagerLibrary.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PswManagerLibrary.Storage {

    /// <summary>
    /// Organizes the storing of the encrypted passwords.
    /// </summary>
    public class PasswordManager : IPasswordManager {

        private readonly CryptoString passCryptoString;
        private readonly CryptoString emaCryptoString;
        private readonly AccountBuilder accBuilder;
        private readonly IPaths paths;

        public PasswordManager(IPaths paths, CryptoString passPassword, CryptoString emaPassword) {
            this.paths = paths;
            this.accBuilder = new AccountBuilder(paths);
            this.passCryptoString = passPassword;
            this.emaCryptoString = emaPassword;
        }

        public void CreatePassword(string name, string password, string email = null) {

            if(AccountExist(name)) {
                throw new InvalidCommandException("The account you're trying to create exists already.");
            }

            //create new account
            File.AppendAllLines(paths.AccountsFilePath, new [] { name });

            //create new password
            File.AppendAllLines(paths.PasswordsFilePath, new[] { passCryptoString.Encrypt(password) });

            //create new email
            File.AppendAllLines(paths.EmailsFilePath, new[] { emaCryptoString.Encrypt(email ?? "") });

        }

        public string GetPassword(string name) {
            
            if(AccountExist(name) is false) {
                throw new InvalidCommandException("The given account doesn't exist.");
            }

            //get values
            var output = accBuilder.GetOne(name);

            //decrypt values
            output.password = passCryptoString.Decrypt(output.password);
            output.email = emaCryptoString.Decrypt(output.email);

            return String.Join(' ', new[] { output.name, output.password, output.email });
        }

        public void EditPassword(string name, string[] arguments) {
            //generate a dictionary with all the possible values
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("name", null);
            values.Add("password", null);
            values.Add("email", null);

            //split keys and values from the arguments
            var splitArgs = arguments.Select(x => x.Split(':'));

            //for every pair, try to insert the value into the dictionary
            foreach(string[] args in splitArgs) {
                if(args.Length != 2) {
                    throw new InvalidCommandException("Invalid format for editing values. The correct argument format is: [key:newValue]. Possible keys: name, password, email.");
                }

                if(values.ContainsKey(args[0])) {
                    values[args[0]] = args[1];

                } else {
                    throw new InvalidCommandException($"Invalid key has been found: {args[0]}");
                
                }
            }

            //if the dictionary is still devoid of values, interrupt the operation for there's nothing to change
            if(values.All(x => x.Value == null)) {
                throw new InvalidCommandException("Lack of valid arguments for the editing process. Please rewrite the command with valid arguments.");
            }

            accBuilder.EditOne(name, values["name"], passCryptoString.Encrypt(values["password"]), emaCryptoString.Encrypt(values["email"]));
        }

        public void DeletePassword(string name) {
            throw new NotImplementedException();
        }

        public bool AccountExist(string name) {
            bool existing = false;

            if(File.Exists(paths.AccountsFilePath)) {
                //check for same-named accounts
                existing = accBuilder.Search(name) is not null;
            }

            return existing;
        }
    }
}
