using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.Interfaces;

/// <summary>
/// Provides methods to get accounts.
/// </summary>
public interface IDataReader : IDataHelper {

    /// <summary>
    /// Enumerates all existing accounts asynchronously.
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync();

    /// <summary>
    /// Attempts to retrieve an account asynchronously. Will return an <see cref="ReaderErrorCode"/> on failure.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<Option<IAccountModel, ReaderErrorCode>> GetAccountAsync(string name);

}