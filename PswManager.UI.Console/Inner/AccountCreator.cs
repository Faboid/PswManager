﻿using PswManager.Core.Services;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using PswManager.Database.Models;
using System.Threading.Tasks;
using PswManager.UI.Console.Inner.Interfaces;

namespace PswManager.UI.Console.Inner;
public class AccountCreator : IAccountCreator {

    public AccountCreator(IDataCreator dataCreator, ICryptoAccountService cryptoAccount) {
        this.dataCreator = dataCreator;
        this.cryptoAccount = cryptoAccount;
    }

    readonly IDataCreator dataCreator;
    readonly ICryptoAccountService cryptoAccount;

    public CreatorResponseCode CreateAccount(IReadOnlyAccountModel model) {
        return CreateAccountAsync(model).GetAwaiter().GetResult();
    }

    public async Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model) {

        var validationResult = model.IsAnyValueNullOrEmpty();
        if(validationResult != ValidationResult.Success) {
            return validationResult switch {
                ValidationResult.MissingName => CreatorResponseCode.InvalidName,
                ValidationResult.MissingPassword => CreatorResponseCode.MissingPassword,
                ValidationResult.MissingEmail => CreatorResponseCode.MissingEmail,
                _ => CreatorResponseCode.Undefined,
            };
        }

        var account = new AccountModel(model.Name, model.Password, model.Email);
        (account.Password, account.Email) = await Task.Run(() => cryptoAccount.Encrypt(account.Password, account.Email)).ConfigureAwait(false);
        return await dataCreator.CreateAccountAsync(account).ConfigureAwait(false);
    }

}

