using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PswManagerLibrary.RefactoringFolder.Storage;
using PswManagerLibrary.RefactoringFolder.Models;
using PswManagerLibrary.Storage;
using PswManagerLibrary.RefactoringFolder.Commands.Validation;

namespace PswManagerLibrary.RefactoringFolder.Commands {
    public class AddCommand : BaseCommand {

        private readonly IPasswordManager pswManager;

        public AddCommand(IPasswordManager pswManager) {
            this.pswManager = pswManager;
        }

        protected override IValidationCollection GetConditions(IValidationCollection collection) {

            collection.AddCommonConditions(3, 3);
            collection.Add((args) => pswManager.AccountExist(args[0]) == false, "The account you're trying to create exists already.");
            //todo - add fake email check

            return collection;
        }

        public override string GetSyntax() {
            return "add [name] [password] [email]";
        }

        protected override CommandResult RunLogic(string[] arguments) {

            pswManager.CreatePassword(arguments[0], arguments[1], arguments[2]);

            return new CommandResult("The account has been created successfully.", true);
        }

    }
}
