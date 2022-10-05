using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner.Interfaces;

/// <summary>
/// Provides methods to retrieve accounts.
/// </summary>
public interface IAccountReader {

    /// <summary>
    /// Tries to retrieve an account named <paramref name="name"/>. Returns <see cref="ReaderErrorCode"/> in case of failure.
    /// </summary>
    /// <remarks>
    /// This is a thin <see cref="Task.Wait"/> wrapper around <see cref="ReadAccountAsync(string)"/>.
    /// </remarks>
    /// <param name="name"></param>
    /// <returns></returns>
    Option<IAccountModel, ReaderErrorCode> ReadAccount(string name);

    /// <summary>
    /// Tries to retrieve an account named <paramref name="name"/> asynchonously. Returns <see cref="ReaderErrorCode"/> in case of failure.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<Option<IAccountModel, ReaderErrorCode>> ReadAccountAsync(string name);

    /// <summary>
    /// Retrieves all existing accounts.
    /// </summary>
    /// <remarks>
    /// This is a thin <see cref="Task.Wait"/> wrapper around <see cref="ReadAllAccountsAsync"/>.
    /// </remarks>
    /// <returns></returns>
    IEnumerable<NamedAccountOption> ReadAllAccounts();

    /// <summary>
    /// Retrieves all existing accounts asynchronously.
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<NamedAccountOption> ReadAllAccountsAsync();

}
