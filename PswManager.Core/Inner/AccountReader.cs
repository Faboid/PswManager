using PswManager.Core.Cryptography;
using PswManager.Core.Inner.Interfaces;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils;
using PswManager.Utils.WrappingObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PswManager.Core.Inner {
    public class AccountReader : IAccountReader {

        private readonly IDataReader dataReader;
        private readonly ICryptoAccount cryptoAccount;

        public AccountReader(IDataReader dataReader, ICryptoAccount cryptoAccount) {
            this.dataReader = dataReader;
            this.cryptoAccount = cryptoAccount;
        }

        public Result<AccountModel> ReadAccount(string name) {
            var result = dataReader.GetAccount(name);
            return Decrypt(result);
        }

        public async Task<Result<AccountModel>> ReadAccountAsync(string name) {
            var result = await dataReader.GetAccountAsync(name).ConfigureAwait(false);
            return await DecryptAsync(result);
        }

        public Result<IEnumerable<Result<AccountModel>>> ReadAllAccounts() {
            var result = dataReader.GetAllAccounts();
            return result.Success switch {
                true => new(result.Value.Select(Decrypt)),
                false => new(result.ErrorMessage)
            };
        }

        public async Task<Result<IAsyncEnumerable<Result<AccountModel>>>> ReadAllAccountsAsync() {
            var result = await dataReader.GetAllAccountsAsync();
            return result.Success switch {
                true => new(result.Value.Select(DecryptAsync)),
                false => new(result.ErrorMessage)
            };
        }

        /// <summary>
        /// If <paramref name="result"/>.Success is:<br/>
        /// <br/><see langword="true"/>: Decrypts the account model and returns a successful result.
        /// <br/><see langword="false"/> Returns a failure result with <paramref name="result"/>.ErrorMessage.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private Result<AccountModel> Decrypt(ConnectionResult<AccountModel> result) => result.Success switch {
            true => new(cryptoAccount.Decrypt(result.Value)),
            false => new(result.ErrorMessage)
        };

        /// <summary>
        /// <see cref="Task.Run{AccountModel}(Func{AccountModel})"/> wrapper of <see cref="Decrypt(ConnectionResult{AccountModel})"/>.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private Task<Result<AccountModel>> DecryptAsync(ConnectionResult<AccountModel> result)
            => Task.Run(() => Decrypt(result));

    }
}
