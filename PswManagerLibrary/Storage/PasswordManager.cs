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

        private readonly CryptoAccount cryptoAccount;
        private readonly AccountBuilder accBuilder;
        private readonly IPaths paths;

        public PasswordManager(IPaths paths, CryptoAccount cryptoAccount) {
            this.paths = paths;
            this.accBuilder = new AccountBuilder(paths);
            this.cryptoAccount = cryptoAccount;
        }

        public void CreatePassword(string name, string password, string email = null) {

            if(AccountExist(name)) {
                throw new InvalidCommandException("The account you're trying to create exists already.");
            }

            //create new account
            File.AppendAllLines(paths.AccountsFilePath, new [] { name });

            //create new password
            File.AppendAllLines(paths.PasswordsFilePath, new[] { cryptoAccount.passCryptoString.Encrypt(password) });

            //create new email
            File.AppendAllLines(paths.EmailsFilePath, new[] { cryptoAccount.emaCryptoString.Encrypt(email ?? "") });

        }

        public string GetPassword(string name) {
            
            if(AccountExist(name) is false) {
                throw new InvalidCommandException("The given account doesn't exist.");
            }

            //get values
            var output = accBuilder.GetOne(name);

            //decrypt values
            (output.password, output.email) = cryptoAccount.Decrypt(output.password, output.email);

            return String.Join(' ', new[] { output.name, output.password, output.email });
        }

        public void EditPassword(string name, string[] arguments) {
            const string nameText = "name";
            const string passText = "password";
            const string emaText = "email";

            //generate a dictionary with all the possible values
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add(nameText, null);
            values.Add(passText, null);
            values.Add(emaText, null);

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

            if(values[passText] is not null) {
                values[passText] = cryptoAccount.passCryptoString.Encrypt(values[passText]);
            }
            if(values[emaText] is not null) {
                values[emaText] = cryptoAccount.emaCryptoString.Encrypt(values[emaText]);
            }

            accBuilder.EditOne(name, values[nameText], values[passText], values[emaText]);
        }

        public void DeletePassword(string name) {
            accBuilder.DeleteOne(name);

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
