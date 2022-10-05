using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.Database.Interfaces;

/// <summary>
/// Provides methods to delete accounts.
/// </summary>
public interface IDataDeleter : IDataHelper {

    /// <summary>
    /// Attempts to delete an account asynchonously and returns the result.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<DeleterResponseCode> DeleteAccountAsync(string name);

}
