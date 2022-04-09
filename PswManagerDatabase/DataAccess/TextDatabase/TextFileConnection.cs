﻿using PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper;
using PswManagerDatabase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.TextDatabase {

    public class TextFileConnection : IDataConnection {

        private readonly IPaths paths;
        private readonly LengthCounter lengthCounter;
        private readonly AccountBuilder accountBuilder;
        private readonly AccountSearcher accountSearcher;

        /// <summary>
        /// Instantiates <see cref="TextFileConnection"/> with the default path provided by <see cref="Paths"/>.
        /// </summary>
        internal TextFileConnection() : this(new Paths()) { }

        /// <summary>
        /// Instantiates <see cref="TextFileConnection"/> with custom paths.
        /// </summary>
        /// <param name="paths">The custom paths where to save the database.</param>
        internal TextFileConnection(IPaths paths) {
            this.paths = paths;
            DatabaseHandler.SetUpMissingFiles(this.paths);
            lengthCounter = new LengthCounter(this.paths);
            accountBuilder = new AccountBuilder(this.paths);
            accountSearcher = new AccountSearcher(this.paths);
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return new ConnectionResult(false, "The given name isn't valid.");
            }
            if(accountSearcher.AccountExist(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            accountBuilder.Create(model);

            lengthCounter.AddOne();
            return new ConnectionResult(true);
        }

        public async Task<ConnectionResult> CreateAccountAsync(AccountModel model) {
            if(string.IsNullOrWhiteSpace(model.Name)) {
                return new ConnectionResult(false, "The given name isn't valid.");
            }
            if(accountSearcher.AccountExist(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            await accountBuilder.CreateAsync(model);
            lengthCounter.AddOne();
            return new ConnectionResult(true);
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(!accountSearcher.AccountExist(name, out int position)) {
                return new ConnectionResult<AccountModel>(false, "The given account doesn't exist.");
            }

            AccountModel output = accountBuilder.Get(position);

            return new ConnectionResult<AccountModel>(true, output);
        }

        public ConnectionResult<IEnumerable<AccountModel>> GetAllAccounts() {

            IEnumerable<AccountModel> accounts = accountBuilder.GetAll();

            return new(true, accounts);
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {

            if(!accountSearcher.AccountExist(name, out int position)) {
                return new ConnectionResult<AccountModel>(false, "The given account doesn't exist.");
            }
            if(name != newModel.Name && AccountExist(newModel.Name)) {
                return new(false, "There is already an account with that name.");
            }

            lengthCounter.ValidateLength(position);

            accountBuilder.Edit(position, newModel);

            return GetAccount(string.IsNullOrWhiteSpace(newModel.Name)? name : newModel.Name);
        }

        public ConnectionResult DeleteAccount(string name) {
            if(!accountSearcher.AccountExist(name, out int position)) {
                return new ConnectionResult(false, "The given account doesn't exist.");
            }

            lengthCounter.ValidateLength(position);

            accountBuilder.Delete(position);

            lengthCounter.SubtractOne();
            return new ConnectionResult(true);
        }

        public bool AccountExist(string name) => accountSearcher.AccountExist(name);

    }
}
