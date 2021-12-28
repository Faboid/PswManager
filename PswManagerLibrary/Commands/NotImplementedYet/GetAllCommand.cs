using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using System;

namespace PswManagerLibrary.Commands.NotImplementedYet {
    public class GetAllCommand : BaseCommand {
        public override string GetSyntax() {
            throw new NotImplementedException();
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            throw new NotImplementedException();
        }

        protected override CommandResult RunLogic(string[] arguments) {
            throw new NotImplementedException();
        }
    }
}
