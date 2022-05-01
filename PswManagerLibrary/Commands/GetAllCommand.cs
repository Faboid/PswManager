using PswManagerCommands;
using PswManagerCommands.AbstractCommands;
using PswManagerCommands.Validation.Builders;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerDatabase.Models;
using PswManagerLibrary.Commands.ArgsModels;
using PswManagerLibrary.Commands.Validation.ValidationTypes;
using PswManagerLibrary.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerLibrary.Commands {
    public class GetAllCommand : BaseCommand<GetAllCommandArgs> {

        readonly IDataReader dataReader;
        readonly ICryptoAccount cryptoAccount;
        readonly string[] validKeys = new string[] { "names", "passwords", "emails" };
        public const string InexistentKeyErrorMessage = "Invalid key(s) has been found. Valid keys: names, passwords, emails.";
        public const string DuplicateKeyErrorMessage = "Duplicate keys aren't allowed.";

        public GetAllCommand(IDataReader dataReader, ICryptoAccount cryptoAccount) {
            this.dataReader = dataReader;
            this.cryptoAccount = cryptoAccount;
        }

        protected override CommandResult RunLogic(GetAllCommandArgs arguments) {
            var result = dataReader.GetAllAccounts();

            if(!result.Success) {
                return new CommandResult($"There has been an error: {result.ErrorMessage}", false);
            }

            if(string.IsNullOrWhiteSpace(arguments.Keys)) {
                return new CommandResult("The list has been retrieved.", true, string.Join(Environment.NewLine, result.Value.Select(Unwrap)));
            }
            bool getNames = arguments.SplitKeys().Contains(validKeys[0]);
            bool getPasswords = arguments.SplitKeys().Contains(validKeys[1]);
            bool getEmails = arguments.SplitKeys().Contains(validKeys[2]);

            var accounts = result.Value
                .Select(x => Unwrap(x, getNames, getPasswords, getEmails));

            return new CommandResult("The list has been retrieved.", true, string.Join(Environment.NewLine, accounts));
        }

        public override string GetDescription() {
            return "Gets a list of the requested parameters of all accounts.";
        }

        protected override AutoValidatorBuilder<GetAllCommandArgs> AddRules(AutoValidatorBuilder<GetAllCommandArgs> builder) => builder
            .AddRule<ValidValuesRule>()
            .AddRule<NoDuplicateValuesRule>();

        private string Unwrap(AccountResult account) {
            return Unwrap(account, true, true, true);
        }

        private string Unwrap(AccountResult result, bool getNames, bool getPasswords, bool getEmails) {
            if(result.Success) {
                var decrypted = Decrypt(result.Value);
                var stringRepresenation = Take(decrypted, getNames, getPasswords, getEmails);
                return Merge(stringRepresenation);
            }

            return $"Error when getting {result.NameAccount}: {result.ErrorMessage ?? "Undefined"}";
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

        private static string Merge(IEnumerable<string> values) => string.Join(' ', values);

    }
}
