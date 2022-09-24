using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Core;
public interface IAccountFactory {
    Task<Option<IAccount, AccountFactory.CreateAccountErrorCode>> CreateAccountAsync(AccountModel model);
    Task<Option<IAsyncEnumerable<IAccount>>> LoadAccounts();
}