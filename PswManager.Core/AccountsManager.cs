using PswManager.Core.Cryptography;
using PswManager.Core.Inner;
using PswManager.Core.Inner.Interfaces;
using PswManager.Database;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Core {
    public class AccountsManager : IAccountDeleter, IAccountEditor, IAccountReader, IAccountCreator {

        public AccountsManager(IDataFactory dbFactory, ICryptoAccount cryptoAccount) {
            accountCreator = new AccountCreator(dbFactory.GetDataCreator(), cryptoAccount);
            accountReader = new AccountReader(dbFactory.GetDataReader(), cryptoAccount);
            accountEditor = new AccountEditor(dbFactory.GetDataEditor(), cryptoAccount);
            accountDeleter = new AccountDeleter(dbFactory.GetDataDeleter());
        }

        //todo - consider whether to use an additional enum
        public AccountsManager(DatabaseType dbType, ICryptoAccount cryptoAccount) {
            var dataFactory = new DataFactory(dbType);
            accountCreator = new AccountCreator(dataFactory.GetDataCreator(), cryptoAccount);
            accountReader = new AccountReader(dataFactory.GetDataReader(), cryptoAccount);
            accountEditor = new AccountEditor(dataFactory.GetDataEditor(), cryptoAccount);
            accountDeleter = new AccountDeleter(dataFactory.GetDataDeleter());
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

        public Option<CreatorErrorCode> CreateAccount(AccountModel model) {
            return accountCreator.CreateAccount(model);
        }

        public Task<Option<CreatorErrorCode>> CreateAccountAsync(AccountModel model) {
            return accountCreator.CreateAccountAsync(model);
        }

        public Option<AccountModel, ReaderErrorCode> ReadAccount(string name) {
            return accountReader.ReadAccount(name);
        }

        public Task<Option<AccountModel, ReaderErrorCode>> ReadAccountAsync(string name) {
            return accountReader.ReadAccountAsync(name);
        }

        public Option<IEnumerable<NamedAccountOption>, ReaderAllErrorCode> ReadAllAccounts() {
            return accountReader.ReadAllAccounts();
        }

        public Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> ReadAllAccountsAsync() {
            return accountReader.ReadAllAccountsAsync();
        }

        public Result UpdateAccount(string name, AccountModel newValues) {
            return accountEditor.UpdateAccount(name, newValues);
        }

        public Task<Result> UpdateAccountAsync(string name, AccountModel newValues) {
            return accountEditor.UpdateAccountAsync(name, newValues);
        }

        public Option<DeleterErrorCode> DeleteAccount(string name) {
            return accountDeleter.DeleteAccount(name);
        }

        public Task<Option<DeleterErrorCode>> DeleteAccountAsync(string name) {
            return accountDeleter.DeleteAccountAsync(name);
        }
    }
}
