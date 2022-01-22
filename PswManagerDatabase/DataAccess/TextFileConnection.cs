﻿using PswManagerDatabase.Config;
using PswManagerDatabase.DataAccess.TextFileConnHelper;
using PswManagerDatabase.Models;
using PswManagerHelperMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PswManagerDatabase.DataAccess {

    public class TextFileConnection : IDataConnection {

        private readonly IPaths paths;
        private readonly LengthCounter lengthCounter;
        private readonly AccountBuilder accountBuilder;

        public TextFileConnection(IPaths paths) {
            this.paths = paths;
            lengthCounter = new LengthCounter(this.paths);
            accountBuilder = new AccountBuilder(this.paths);
        }

        public IPaths GetPaths() {
            return paths;
        }

        /// <summary>
        /// Returns the position of the name. If it doesn't find any, returns null.
        /// </summary>
        private int? SearchByName(string name) {
            int position = 0;

            using(StreamReader reader = new StreamReader(paths.AccountsFilePath)) {
                string current;
                while((current = reader.ReadLine()) != name) {
                    position++;

                    if(current is null) {
                        return null;
                    }
                }
            }

            return position;
        }

        public ConnectionResult CreateAccount(AccountModel model) {
            if(AccountExist(model.Name)) {
                return new ConnectionResult(false, "The given account name is already occupied.");
            }

            accountBuilder.Create(model);

            lengthCounter.AddOne();
            return new ConnectionResult(true);
        }

        public ConnectionResult<AccountModel> GetAccount(string name) {
            if(!AccountExist(name, out int position)) {
                return new ConnectionResult<AccountModel>(false, "The given account doesn't exist.");
            }

            AccountModel output = accountBuilder.Get(position);

            return new ConnectionResult<AccountModel>(true, output);
        }

        public ConnectionResult<List<AccountModel>> GetAllAccounts() {

            List<AccountModel> accounts = accountBuilder.GetAll();

            return new ConnectionResult<List<AccountModel>>(true, accounts);
        }

        public ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel) {

            if(!AccountExist(name, out int position)) {
                return new ConnectionResult<AccountModel>(false, "The given account doesn't exist.");
            }

            lengthCounter.ValidateLength(position);

            accountBuilder.Edit(position, newModel);

            return GetAccount(newModel.Name ?? name);
        }

        public ConnectionResult DeleteAccount(string name) {
            if(!AccountExist(name, out int position)) {
                return new ConnectionResult(false, "The given account doesn't exist.");
            }

            lengthCounter.ValidateLength(position);

            accountBuilder.Delete(position);

            lengthCounter.SubtractOne();
            return new ConnectionResult(true);
        }

        public bool AccountExist(string name) {
            return File.Exists(paths.AccountsFilePath) && SearchByName(name) != null;
        }

        private bool AccountExist(string name, out int position) {
            position = -1;

            if(!File.Exists(paths.AccountsFilePath)) {
                return false;
            }

            int? temp = SearchByName(name);
            if(temp == null)
                return false;

            position = (int)temp;

            return true;
        }

    }
}
