using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.Database.Interfaces;

/// <summary>
/// Provides methods to check whether the account exists.
/// </summary>
public interface IDataHelper {

    /// <summary>
    /// Checks whether the account exists.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    AccountExistsStatus AccountExist(string name);

    /// <summary>
    /// Checks whether the account exists asynchonously.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<AccountExistsStatus> AccountExistAsync(string name);

}
