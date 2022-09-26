using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner.Interfaces;
public interface IAccountReader {

    Option<AccountModel, ReaderErrorCode> ReadAccount(string name);
    Task<Option<AccountModel, ReaderErrorCode>> ReadAccountAsync(string name);

    Option<IEnumerable<NamedAccountOption>, ReaderAllErrorCode> ReadAllAccounts();
    Task<Option<IAsyncEnumerable<NamedAccountOption>, ReaderAllErrorCode>> ReadAllAccountsAsync();

}
