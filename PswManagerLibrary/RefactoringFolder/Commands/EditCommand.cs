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
            collection.AddCommonConditions(2, 4);
            collection.AddAccountShouldExistCondition(pswManager);
            //todo - add fake email check

            return collection.Get();
        }

        protected override CommandResult RunLogic(string[] arguments) {
            pswManager.EditPassword(arguments[0], arguments.Skip(1).ToArray());

            return new CommandResult("The account has been edited successfully.", true);
        }
    }
}
