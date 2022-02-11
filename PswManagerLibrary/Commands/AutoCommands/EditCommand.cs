using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Commands.AutoCommands.ArgsModels;
using PswManagerLibrary.Commands.Validation.ValidationLogic;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerLibrary.Commands.AutoCommands {
    public sealed class EditCommand : AutoCommand<EditAccountModel> {

        private readonly IDataEditor dataEditor;
        private readonly ICryptoAccount cryptoAccount;
        public const string InvalidSyntaxMessage = "Invalid syntax used for this command. For more informations, run [help edit]";
        public const string InvalidKeyFound = "Invalid key(s) has been found. Valid keys: name, password, email.";
        public const string DuplicateKeyFound = "Duplicate keys aren't allowed.";

        private record SyntaxCheckResult {
            public bool ValidSyntax { get; set; } = true;
            public bool ValidKeys { get; set; } = true;
            public bool NoDuplicateKeys { get; set; } = true;
        }

        public EditCommand(IDataEditor dataEditor, ICryptoAccount cryptoAccount) {
            this.dataEditor = dataEditor;
            this.cryptoAccount = cryptoAccount;
        }

        public override string GetDescription() {
            return "Edits an existing account with the provided arguments.";
        }

        public override string GetSyntax() {
            return "edit [name] name:[new name]? password:[new password]? email:[new email]?";
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            collection.AddCommonConditions(2, 4);
            collection.AddAccountShouldExistCondition(0, dataEditor);

            if(collection.IndexesAreValid(collection.NullIndexCondition, collection.NullOrEmptyArgsIndexCondition, collection.CorrectArgsNumberIndexCondition)) {

                var syntaxCheckResult = CheckSyntax(collection.GetArguments());
                collection.Add(1, syntaxCheckResult.ValidSyntax, InvalidSyntaxMessage);
                collection.Add(2, syntaxCheckResult.ValidKeys, InvalidKeyFound);
                collection.Add(3, syntaxCheckResult.NoDuplicateKeys, DuplicateKeyFound);
            }

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            string name = arguments[0];
            AccountModel newValues = new();
            var keys = arguments
                .Skip(1)
                .Select(x => x.Split(':').First().ToLowerInvariant())
                .ToArray();
            var values = arguments
                .Skip(1)
                .Select(x => x.Split(':').Skip(1))
                .Select(x => string.Join(':', x))
                .ToArray();

            for(int i = 0; i < keys.Length; i++) {
                switch(keys[i]) {
                    case "name":
                        newValues.Name = values[i];
                        break;
                    case "password":
                        newValues.Password = cryptoAccount.GetPassCryptoString().Encrypt(values[i]);
                        break;
                    case "email":
                        newValues.Email = cryptoAccount.GetEmaCryptoString().Encrypt(values[i]);
                        break;
                    default:
                        return new CommandResult($"Operation failed. The key [{keys[i]}] is invalid.", false);
                }
            }

            dataEditor.UpdateAccount(arguments[0], newValues);

            return new CommandResult("The account has been edited successfully.", true);
        }

        private static SyntaxCheckResult CheckSyntax(in string[] args) {
            var result = new SyntaxCheckResult();

            try {
                var argsToTest = args.Skip(1); //skips the name

                if(argsToTest.Any(x => !x.Contains(':'))) {
                    result.ValidSyntax = false;
                    return result;
                }

                //string key, bool isUsed
                Dictionary<string, bool> keys = GetValidKeys();

                var givenKeyArguments = argsToTest.Select(x => x.Split(':').First());

                CheckKeys(ref result, keys, givenKeyArguments);
            }
            catch(Exception) {
                result.ValidSyntax = false;
            }

            return result;
        }

        private static void CheckKeys(ref SyntaxCheckResult result, Dictionary<string, bool> keys, IEnumerable<string> givenKeyArguments) {

            foreach(string s in givenKeyArguments) {

                if(keys.ContainsKey(s)) {

                    if(keys[s] == true) {
                        result.NoDuplicateKeys = false;
                    }
                    keys[s] = true;

                }
                else {
                    result.ValidKeys = false;
                }
            }
        }

        private static Dictionary<string, bool> GetValidKeys() {
            Dictionary<string, bool> keys = new();
            keys.Add("name", false);
            keys.Add("password", false);
            keys.Add("email", false);
            return keys;
        }

        protected override AutoValidation<EditAccountModel> BuildAutoValidator(AutoValidationBuilder<EditAccountModel> builder) {
            return builder
                .AddLogic(new VerifyAccountExistenceLogic(dataEditor))
                .Build();
        }

    }
}
