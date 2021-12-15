using PswManagerLibrary.RefactoringFolder.Commands.Validation;
using PswManagerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder.Commands {
    public class EditCommand : BaseCommand {
        //todo - implement tests for this command
        private readonly IPasswordManager pswManager;

        public EditCommand(IPasswordManager pswManager) {
            this.pswManager = pswManager;
        }

        public override string GetSyntax() {
            return "edit [name] name:[new name]? password:[new password]? email:[new email]?";
        }

        protected override IReadOnlyList<(bool condition, string errorMessage)> GetConditions(string[] args) {
            ValidationCollection collection = new ValidationCollection(args);
            collection.Add(args != null, "The arguments cannot be null.");
            collection.Add((args) => args.Length >= 2 && args.Length <= 4, "Incorrect arguments number.");
            collection.Add((args) => pswManager.AccountExist(args[0]) == true, "The given account doesn't exist.");
            collection.Add((args) => args.All(x => string.IsNullOrWhiteSpace(x) == false), "No value can be left empty.");
            //todo - add fake email check

            return collection.Get();
        }

        protected override CommandResult RunLogic(string[] arguments) {
            pswManager.EditPassword(arguments[0], arguments.Skip(1).ToArray());

            return new CommandResult("The account has been edited successfully.", true);
        }
    }
}
