using PswManagerLibrary.RefactoringFolder.Commands.Validation;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Global;
using PswManagerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder.Commands {
    public class AddCommand : BaseCommand {

        private readonly ICryptoAccount cryptoAccount;
        private readonly IPaths paths;
        private readonly AccountBuilder accBuilder;

        public AddCommand() {

        }

        protected override IReadOnlyList<ConditionValidator> GetConditions() {
            List<ConditionValidator> conditions = new();
            conditions.Add(new ConditionValidator((string[] args) => { return args.Length == 3; }, "Incorrect arguments number."));

            return conditions.AsReadOnly();
        }

        public override string GetSyntax() {
            return "add [name] [password] [email]";
        }

        protected override (string message, string value) RunLogic(string[] arguments) {
            string name = arguments[0];
            string password = arguments[1];
            string email = arguments[2];

            //create new account
            File.AppendAllLines(paths.AccountsFilePath, new[] { name });

            //create new password
            File.AppendAllLines(paths.PasswordsFilePath, new[] { cryptoAccount.GetPassCryptoString().Encrypt(password) });

            //create new email
            File.AppendAllLines(paths.EmailsFilePath, new[] { cryptoAccount.GetEmaCryptoString().Encrypt(email ?? "") });


            throw new NotImplementedException();
        }

        private bool AccountExist(string name) {
            bool existing = false;

            if(File.Exists(paths.AccountsFilePath)) {
                //check for same-named accounts
                existing = accBuilder.Search(name) is not null;
            }

            return existing;
        }
    }
}
