using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Database.Interfaces;

/// <summary>
/// Provides methods to create an account.
/// </summary>
public interface IDataCreator : IDataHelper {

    /// <summary>
    /// Attempts to create an account asynchonously and returns the result.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model);

}
