using PswManager.Core.AccountModels;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Core;

/// <summary>
/// Provides methods to interact with <see cref="IAccount"/> and the database.
/// </summary>
public interface IAccountFactory {

    /// <summary>
    /// Tries to create a new account. Will save it to DB and return <see cref="IAccount"/> on success. On failure, returns <see cref="IAccountFactory.CreateAccountErrorCode"/>.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<Option<IAccount, CreateAccountErrorCode>> CreateAccountAsync(IExtendedAccountModel model);

    //todo - once corrupted accounts are treated as valid values, update this xml
    /// <summary>
    /// Loads the accounts from DB and returns the non-corrupted ones.
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<IAccount> LoadAccounts();

    public enum CreateAccountErrorCode {
        Unknown,
        NameEmptyOrNull,
        PasswordEmptyOrNull,
        EmailEmptyOrNull,
        NameIsOccupied,
    }

}