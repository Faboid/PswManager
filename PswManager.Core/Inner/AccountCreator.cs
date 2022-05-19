using PswManager.Core.Cryptography;
using PswManager.Core.Inner.Interfaces;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;
using System.Threading.Tasks;

namespace PswManager.Core.Inner {
    public class AccountCreator : IAccountCreator {

        public AccountCreator(IDataCreator dataCreator, ICryptoAccount cryptoAccount) {
            this.dataCreator = dataCreator;
            this.cryptoAccount = cryptoAccount;
        }

        readonly IDataCreator dataCreator;
        readonly ICryptoAccount cryptoAccount;

        readonly Result nullOrWhiteSpaceNameResult = new("You must provide a valid account name.");
        readonly Result nullOrWhiteSpacePasswordResult = new("You must provide a valid password.");
        readonly Result nullOrWhiteSpaceEmailResult = new("You must provide a valid email.");
        readonly Result alreadyExistingAccountResult = new($"The given account exists already.");

        public Result CreateAccount(AccountModel model) {

            if(IsAnyValueNullOrEmpty(model, out var failResult)) {
                return failResult;
            }

            if(dataCreator.AccountExist(model.Name)) {
                return alreadyExistingAccountResult;
            }

            (model.Password, model.Email) = cryptoAccount.Encrypt(model.Password, model.Email);
            var account = new AccountModel(model.Name, model.Password, model.Email);
            var result = dataCreator.CreateAccount(account);
            return ConnectionResultToResult(result);
        }

        public async Task<Result> CreateAccountAsync(AccountModel model) {

            if(IsAnyValueNullOrEmpty(model, out var failResult)) {
                return failResult;
            }

            if(await dataCreator.AccountExistAsync(model.Name)) {
                return alreadyExistingAccountResult;
            }

            (model.Password, model.Email) = await Task.Run(() => cryptoAccount.Encrypt(model.Password, model.Email)).ConfigureAwait(false);
            var account = new AccountModel(model.Name, model.Password, model.Email);
            var result = await dataCreator.CreateAccountAsync(account).ConfigureAwait(false);
            return ConnectionResultToResult(result);
        }

        //might want to create either an extension or an implicit conversion
        private static Result ConnectionResultToResult(ConnectionResult connResult) {
            return connResult.Success switch {
                true => new Result(true),
                false => new Result(connResult.ErrorMessage)
            };
        }

        //todo - experiment with struct-based validation to get rid of this boilerplate
        private bool IsAnyValueNullOrEmpty(AccountModel account, out Result failureResult) {

            if(string.IsNullOrWhiteSpace(account.Name)) {
                failureResult = nullOrWhiteSpaceNameResult;
                return true;
            }

            if(string.IsNullOrWhiteSpace(account.Password)) {
                failureResult = nullOrWhiteSpacePasswordResult;
                return true;
            }

            if(string.IsNullOrWhiteSpace(account.Email)) {
                failureResult = nullOrWhiteSpaceEmailResult;
                return true;
            }

            failureResult = null;
            return false;

        }

    }

}
