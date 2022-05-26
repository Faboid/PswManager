﻿using PswManager.Core.Cryptography;
using PswManager.Core.Inner.Interfaces;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;
using System.Threading.Tasks;

namespace PswManager.Core.Inner {
    public class AccountEditor : IAccountEditor {

        private readonly IDataEditor dataEditor;
        private readonly ICryptoAccount cryptoAccount;

        private readonly Result<AccountModel> inexistentAccountResult = new("The account you're trying to edit does not exist.");
        private readonly Result<AccountModel> invalidNameResult = new("The given name is empty or null.");
        private readonly Result<AccountModel> newNameIsOccupiedResult = new("The new name you've given is already used.");

        public AccountEditor(IDataEditor dataEditor, ICryptoAccount cryptoAccount) {
            this.dataEditor = dataEditor;
            this.cryptoAccount = cryptoAccount;
        }

        public Result<AccountModel> UpdateAccount(string name, AccountModel newValues) {

            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            if(!dataEditor.AccountExist(name)) {
                return inexistentAccountResult;
            }

            if(name != newValues.Name && dataEditor.AccountExist(newValues.Name)) {
                return newNameIsOccupiedResult;
            }

            EncryptModel(newValues);
            var result = dataEditor.UpdateAccount(name, newValues);

            return result.Success switch {
                true => new (true),
                false => new ($"There has been an error: {result.ErrorMessage}")
            };
        }

        public async Task<Result<AccountModel>> UpdateAccountAsync(string name, AccountModel newValues) {

            if(string.IsNullOrWhiteSpace(name)) {
                return invalidNameResult;
            }

            if(!await dataEditor.AccountExistAsync(name)) {
                return inexistentAccountResult;
            }

            if(name != newValues.Name && await dataEditor.AccountExistAsync(newValues.Name)) {
                return newNameIsOccupiedResult;
            }


            await Task.Run(() => EncryptModel(newValues)).ConfigureAwait(false);
            var result = await dataEditor.UpdateAccountAsync(name, newValues).ConfigureAwait(false);

            return result.Success switch {
                true => new(true),
                false => new($"There has been an error: {result.ErrorMessage}")
            };
        }

        private void EncryptModel(AccountModel args) {
            if(!string.IsNullOrWhiteSpace(args.Password)) {
                args.Password = cryptoAccount.GetPassCryptoService().Encrypt(args.Password);
            }
            if(!string.IsNullOrWhiteSpace(args.Email)) {
                args.Email = cryptoAccount.GetEmaCryptoService().Encrypt(args.Email);
            }
        }

    }
}