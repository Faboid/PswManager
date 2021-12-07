using PswManagerLibrary.RefactoringFolder.Commands.Validation;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PswManagerLibrary.RefactoringFolder.Storage;
using PswManagerLibrary.RefactoringFolder.Models;
using PswManagerLibrary.Storage;

namespace PswManagerLibrary.RefactoringFolder.Commands {
    public class AddCommand : BaseCommand {

        private readonly IPasswordManager pswManager;

        public AddCommand(IPasswordManager pswManager) {
            this.pswManager = pswManager;
        }

        protected override IReadOnlyList<ConditionValidator> GetConditions() {
            List<ConditionValidator> conditions = new() {
                new ConditionValidator((string[] args) => { return args.Length == 3; }, "Incorrect arguments number."),
                new ConditionValidator((string[] args) => { return pswManager.AccountExist(args[0]); }, "The account you're trying to create exists already.")
            };

            return conditions.AsReadOnly();
        }

        public override string GetSyntax() {
            return "add [name] [password] [email]";
        }

        protected override (string message, string value) RunLogic(string[] arguments) {

            pswManager.CreatePassword(arguments[0], arguments[1], arguments[2]);

            return ("The account has been created successfully.", null);
        }

    }
}
