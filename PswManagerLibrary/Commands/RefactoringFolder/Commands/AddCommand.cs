using PswManagerLibrary.Commands.RefactoringFolder.Commands.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.RefactoringFolder.Commands {
    public class AddCommand : BaseCommand {

        protected override IReadOnlyList<ConditionValidator> GetConditions() {
            List<ConditionValidator> conditions = new();
            conditions.Add(new ConditionValidator((string[] args) => { return args.Length == 3; }, "Incorrect arguments number."));

            return conditions.AsReadOnly();
        }

        public override string GetSyntax() {
            return "add [name] [password] [email]";
        }

        protected override (string message, string value) RunLogic(string[] arguments) {
            throw new NotImplementedException();
        }

    }
}
