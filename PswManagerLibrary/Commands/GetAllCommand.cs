using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase.DataAccess.Interfaces;
using System;
using System.Linq;

namespace PswManagerLibrary.Commands {
    public class GetAllCommand : BaseCommand {

        readonly IDataReader dataReader;

        public GetAllCommand(IDataReader dataReader) {
            this.dataReader = dataReader;
        }

        public override string GetDescription() {
            return "Gets a list with all account names.";
        }

        public override string GetSyntax() {
            return "get-all";
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            collection.AddCommonConditions(0, 0);

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            var result = dataReader.GetAllAccounts();
            var names = result.Value.Select(x => x.Name);

            return new CommandResult("The list has been retrieved.", true, string.Join(' ', names));
        }
    }
}
