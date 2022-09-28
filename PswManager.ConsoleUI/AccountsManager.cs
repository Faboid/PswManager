using PswManager.ConsoleUI.Inner;
using PswManager.ConsoleUI.Inner.Interfaces;
using PswManager.Core.Services;
using PswManager.Database;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI;
public class AccountsManager : IAccountsManager {

    public AccountsManager(IDataFactory dbFactory, ICryptoAccountService cryptoAccount) {
        accountCreator = new AccountCreator(dbFactory.GetDataCreator(), cryptoAccount);
        accountReader = new AccountReader(dbFactory.GetDataReader(), cryptoAccount);
        accountEditor = new AccountEditor(dbFactory.GetDataEditor(), cryptoAccount);
        accountDeleter = new AccountDeleter(dbFactory.GetDataDeleter());
    }

    //todo - consider whether to use an additional enum
    public AccountsManager(DatabaseType dbType, ICryptoAccountService cryptoAccount) {
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

    public CreatorResponseCode CreateAccount(IReadOnlyAccountModel model) {
        return accountCreator.CreateAccount(model);
    }

    public Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model) {
        return accountCreator.CreateAccountAsync(model);
    }

    public Option<IAccountModel, ReaderErrorCode> ReadAccount(string name) {
        return accountReader.ReadAccount(name);
    }

    public Task<Option<IAccountModel, ReaderErrorCode>> ReadAccountAsync(string name) {
        return accountReader.ReadAccountAsync(name);
    }

    public IEnumerable<NamedAccountOption> ReadAllAccounts() {
        return accountReader.ReadAllAccounts();
    }

    public IAsyncEnumerable<NamedAccountOption> ReadAllAccountsAsync() {
        return accountReader.ReadAllAccountsAsync();
    }

    public EditorResponseCode UpdateAccount(string name, IReadOnlyAccountModel newValues) {
        return accountEditor.UpdateAccount(name, newValues);
    }

    public Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newValues) {
        return accountEditor.UpdateAccountAsync(name, newValues);
    }

    public DeleterResponseCode DeleteAccount(string name) {
        return accountDeleter.DeleteAccount(name);
    }

    public Task<DeleterResponseCode> DeleteAccountAsync(string name) {
        return accountDeleter.DeleteAccountAsync(name);
    }

}
