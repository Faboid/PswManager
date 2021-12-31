using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerLibrary.Extensions;
using PswManagerLibrary.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PswManagerLibrary.Commands {
    public class EditCommand : BaseCommand {

        private readonly IPasswordManager pswManager;
        public const string InvalidSyntaxMessage = "Invalid syntax used for this command. For more informations, run [help edit]";
        public const string InvalidKeyFound = "Invalid key has been found:";

        public EditCommand(IPasswordManager pswManager) {
            this.pswManager = pswManager;
        }

        public override string GetSyntax() {
            return "edit [name] name:[new name]? password:[new password]? email:[new email]?";
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            collection.AddCommonConditions(2, 4);
            collection.AddAccountShouldExistCondition(pswManager);
            collection.Add(CheckSyntax, InvalidSyntaxMessage);
            //todo - add check for invalid keys

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            pswManager.EditPassword(arguments[0], arguments.Skip(1).ToArray());

            return new CommandResult("The account has been edited successfully.", true);
        }

        private bool CheckSyntax(string[] args) {
            var toTest = args.Skip(1); //skips the name
            string[] types = new string[] { "name", "password", "email" };

            int corretCount = 0;
            foreach(string type in types) {
                if(CheckRegex(toTest, type)) {
                    corretCount++;
                }
            }

            return corretCount == toTest.Count();
        }

        private bool CheckRegex(IEnumerable<string> args, string constantSide) {
            return args.Any(x => GetRegex(constantSide).IsMatch(x));
        }

        private Regex GetRegex(string constantSide) {
            return new Regex($"^{constantSide}:");
        }
    }
}
