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

        readonly Result alreadyExistingAccountResult = new($"The given account exists already.");

        public Result CreateAccount(AccountModel model) {

            if(model.IsAnyValueNullOrEmpty(out var failResult)) {
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

            if(model.IsAnyValueNullOrEmpty(out var failResult)) {
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

    }

}
