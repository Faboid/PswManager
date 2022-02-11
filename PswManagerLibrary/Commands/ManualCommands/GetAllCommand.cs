using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerLibrary.Commands.ManualCommands {
    public class GetAllCommand : ManualCommand {

        readonly IDataReader dataReader;
        readonly ICryptoAccount cryptoAccount;
        readonly string[] validKeys = new string[] { "names", "passwords", "emails" };
        public const string InexistentKeyErrorMessage = "Invalid key(s) has been found. Valid keys: names, passwords, emails.";
        public const string DuplicateKeyErrorMessage = "Duplicate keys aren't allowed.";

        public GetAllCommand(IDataReader dataReader, ICryptoAccount cryptoAccount) {
            this.dataReader = dataReader;
            this.cryptoAccount = cryptoAccount;
        }

        public override string GetDescription() {
            return "Gets a list of the requested parameters of all accounts.";
        }

        public override string GetSyntax() {
            return $"get-all { string.Join(' ', validKeys.Select(x => $"[{x}]?")) }";
        }

        protected override IValidationCollection AddConditions(IValidationCollection collection) {
            collection.AddCommonConditions(0, validKeys.Length);
            collection.Add(new IndexHelper(0, collection.NullOrEmptyArgsIndexCondition), (args) => args.All(x => validKeys.Contains(x)), InexistentKeyErrorMessage);
            collection.Add(new IndexHelper(1, 0), (args) => args.Distinct().Count() == args.Length, DuplicateKeyErrorMessage);

            return collection;
        }

        protected override CommandResult RunLogic(string[] arguments) {
            var result = dataReader.GetAllAccounts();

            if(arguments.Length == 0) {
                return new CommandResult("The list has been retrieved.", true, string.Join(Environment.NewLine, result.Value.Select(Decrypt)));
            }
            bool getNames = arguments.Contains(validKeys[0]);
            bool getPasswords = arguments.Contains(validKeys[1]);
            bool getEmails = arguments.Contains(validKeys[2]);

            var accounts = result.Value
                .Select(Decrypt)
                .Select(x => Take(x, getNames, getPasswords, getEmails).ToArray())
                .Select(Merge);

            return new CommandResult("The list has been retrieved.", true, string.Join(Environment.NewLine, accounts));
        }

        private AccountModel Decrypt(AccountModel account) {
            (account.Password, account.Email) = cryptoAccount.Decrypt(account.Password, account.Email);
            return account;
        }

        private static IEnumerable<string> Take(AccountModel account, bool takeName, bool takePassword, bool takeEmail) {
            if(takeName)
                yield return account.Name;
            if(takePassword)
                yield return account.Password;
            if(takeEmail)
                yield return account.Email;
        }

        private static string Merge(string[] values) => string.Join(' ', values);

    }
}
