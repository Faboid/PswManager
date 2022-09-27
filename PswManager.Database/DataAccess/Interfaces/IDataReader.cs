using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces; 
public interface IDataReader : IDataHelper {

    IAsyncEnumerable<NamedAccountOption> GetAllAccountsAsync();
    Task<Option<AccountModel, ReaderErrorCode>> GetAccountAsync(string name);

}

