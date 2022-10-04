using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner.Interfaces;
public interface IAccountReader {

    Option<IAccountModel, ReaderErrorCode> ReadAccount(string name);
    Task<Option<IAccountModel, ReaderErrorCode>> ReadAccountAsync(string name);

    IEnumerable<NamedAccountOption> ReadAllAccounts();
    IAsyncEnumerable<NamedAccountOption> ReadAllAccountsAsync();

}
