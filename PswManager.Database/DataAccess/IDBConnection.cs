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
    Task<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name);
    IAsyncEnumerable<NamedAccountOption> EnumerateAccountsAsync(NamesLocker locker);
}
