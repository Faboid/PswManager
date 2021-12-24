using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Commands.NotImplementedYet {
    public class ChangeDatabaseLocationCommand : BaseCommand {
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
