﻿using PswManager.Core.Inner.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Core {
    public class AccountsManager : IAccountDeleter, IAccountEditor, IAccountReader, IAccountCreator {

        //introduce enum to choose db types(maybe?)
        public AccountsManager() {
            
        }

        internal AccountsManager(IAccountCreator accountCreator, IAccountReader accountReader, IAccountEditor accountEditor, IAccountDeleter accountDeleter) {
            this.accountCreator = accountCreator;
            this.accountReader = accountReader;
            this.accountEditor = accountEditor;
            this.accountDeleter = accountDeleter;
        }

        readonly IAccountCreator accountCreator;
        readonly IAccountReader accountReader;
        readonly IAccountEditor accountEditor;
        readonly IAccountDeleter accountDeleter;

        public Result CreateAccount(AccountModel model) {
            return accountCreator.CreateAccount(model);
        }

        public Task<Result> CreateAccountAsync(AccountModel model) {
            return accountCreator.CreateAccountAsync(model);
        }

        public Result<AccountModel> ReadAccount(string name) {
            return accountReader.ReadAccount(name);
        }

        public Task<Result<AccountModel>> ReadAccountAsync(string name) {
            return accountReader.ReadAccountAsync(name);
        }

        public Result<IEnumerable<AccountModel>> ReadAllAccounts() {
            return accountReader.ReadAllAccounts();
        }

        public Task<Result<IAsyncEnumerable<AccountModel>>> ReadAllAccountsAsync() {
            return accountReader.ReadAllAccountsAsync();
        }

        public Result<AccountModel> UpdateAccount(string name, AccountModel newValues) {
            return accountEditor.UpdateAccount(name, newValues);
        }

        public Task<Result<AccountModel>> UpdateAccountAsync(string name, AccountModel newValues) {
            return accountEditor.UpdateAccountAsync(name, newValues);
        }

        public Result DeleteAccount(string name) {
            return accountDeleter.DeleteAccount(name);
        }

        public Task<Result> DeleteAccountAsync(string name) {
            return accountDeleter.DeleteAccountAsync(name);
        }
    }
}
