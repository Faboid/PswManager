using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Cryptography;
using PswManagerLibrary.UIConnection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerLibrary.Commands {
    public class GetAllCommand : BaseCommand<GetAllCommand.KeyArgs> {

        readonly IDataReader dataReader;
        readonly ICryptoAccount cryptoAccount;
        readonly string[] validKeys = new string[] { "names", "passwords", "emails" };
        public const string InexistentKeyErrorMessage = "Invalid key(s) has been found. Valid keys: names, passwords, emails.";
        public const string DuplicateKeyErrorMessage = "Duplicate keys aren't allowed.";

        public class KeyArgs : ICommandInput {

            [Request("Keys", true, "Leave empty if you want all values.", 
                "If you only want specific ones, insert their keys: names, passwords, emails.",
                "Properly put a single space between the keys, if you require multiple.")]
            public string Keys { get; private set; }

            public IEnumerable<string> SplitKeys() => Keys.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x));

        }

        public GetAllCommand(IDataReader dataReader, ICryptoAccount cryptoAccount) {
            this.dataReader = dataReader;
            this.cryptoAccount = cryptoAccount;
        }

        public override string GetDescription() {
            return "Gets a list of the requested parameters of all accounts.";
        }

        protected override IValidationCollection<KeyArgs> AddConditions(IValidationCollection<KeyArgs> collection) {
            var obj = collection.GetObject();

            collection.Add(0, obj.SplitKeys().All(x => validKeys.Contains(x)), InexistentKeyErrorMessage);
            collection.Add(1, obj.SplitKeys().Distinct().Count() == obj.SplitKeys().Count(), DuplicateKeyErrorMessage);

            return collection;
        }

        protected override CommandResult RunLogic(KeyArgs arguments) {
            var result = dataReader.GetAllAccounts();

            if(string.IsNullOrWhiteSpace(arguments.Keys)) { 
                return new CommandResult("The list has been retrieved.", true, string.Join(Environment.NewLine, result.Value.Select(Decrypt)));
            }
            bool getNames = arguments.SplitKeys().Contains(validKeys[0]);
            bool getPasswords = arguments.SplitKeys().Contains(validKeys[1]);
            bool getEmails = arguments.SplitKeys().Contains(validKeys[2]);

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
            if(takeName) yield return account.Name;
            if(takePassword) yield return account.Password;
            if(takeEmail) yield return account.Email;
        }

        private static string Merge(string[] values) => string.Join(' ', values);

    }
}
