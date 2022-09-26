using PswManager.Core.AccountModels;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Core;
public interface IAccountFactory {
    Task<Option<IAccount, AccountFactory.CreateAccountErrorCode>> CreateAccountAsync(IAccountModel model);
    Task<Option<IAsyncEnumerable<IAccount>>> LoadAccounts();
}