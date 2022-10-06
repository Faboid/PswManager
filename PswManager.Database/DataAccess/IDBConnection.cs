using PswManager.Async.Locks;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess;

/// <summary>
/// An internal database connection. Does not validate anything by itself; to use with the wrappers at <see cref="Wrappers"/>.
/// </summary>
internal interface IDBConnection : IDataHelper, IDataCreator, IDataEditor, IDataDeleter {

    /// <summary>
    /// Attempts to get the account asynchonously.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>The account if successful, else, a <see cref="ReaderErrorCode"/> with the error.</returns>
    Task<Option<IAccountModel, ReaderErrorCode>> GetAccountAsync(string name);

    /// <summary>
    /// Enumerates all existing accounts.
    /// </summary>
    /// <param name="locker"></param>
    /// <returns></returns>
    IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync(NamesLocker locker);
}
