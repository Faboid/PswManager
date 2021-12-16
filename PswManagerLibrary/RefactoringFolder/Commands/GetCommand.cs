using PswManagerLibrary.RefactoringFolder.Commands.Validation;
using PswManagerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder.Commands {
    public class GetCommand : BaseCommand {

        private readonly IPasswordManager pswManager;

        public GetCommand(IPasswordManager pswManager) {
            this.pswManager = pswManager;
        }

        public override string GetSyntax() {
            return "get [name]";
        }

        protected override IReadOnlyList<(bool condition, string errorMessage)> GetConditions(string[] arguments) {

            ValidationCollection collection = new(arguments);
            collection.AddCommonConditions(1, 1);
            collection.AddAccountShouldExistCondition(pswManager);

            return collection.Get();
        }

        protected override CommandResult RunLogic(string[] arguments) {
            var value = pswManager.GetPassword(arguments[0]);

            return new CommandResult("The account has been retrieved successfully.", true, value);
        }
    }
}
