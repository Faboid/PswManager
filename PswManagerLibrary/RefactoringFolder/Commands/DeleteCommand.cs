using PswManagerLibrary.RefactoringFolder.Commands.Validation;
using PswManagerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder.Commands {
    public class DeleteCommand : BaseCommand {

        //todo - implement tests for this command
        private readonly IPasswordManager pswManager;

        public DeleteCommand(IPasswordManager pswManager) {
            this.pswManager = pswManager;
        }

        public override string GetSyntax() {
            return "delete [name]";
        }

        protected override IReadOnlyList<(bool condition, string errorMessage)> GetConditions(string[] args) {
            ValidationCollection collection = new ValidationCollection(args);
            collection.Add(args != null, "The arguments cannot be null.");
            collection.Add((args) => args.Length == 1, "Incorrect arguments number.");
            collection.Add((args) => pswManager.AccountExist(args[0]) == true, "The given account doesn't exist.");
            collection.Add((args) => string.IsNullOrWhiteSpace(args[0]) == false, "The name cannot be left empty.");

            return collection.Get();
        }

        protected override CommandResult RunLogic(string[] arguments) {
            pswManager.DeletePassword(arguments[0]);

            return new CommandResult("Account deleted successfully.", true);
        }
    }
}
