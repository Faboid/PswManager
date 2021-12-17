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

        protected override IValidationCollection GetConditions(IValidationCollection collection) {
            collection.AddCommonConditions(1, 1);
            collection.AddAccountShouldExistCondition(pswManager);

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            pswManager.DeletePassword(arguments[0]);

            return new CommandResult("Account deleted successfully.", true);
        }
    }
}
