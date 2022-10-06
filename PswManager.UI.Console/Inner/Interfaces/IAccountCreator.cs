using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.UI.Console.Inner.Interfaces;

/// <summary>
/// Provides methods to create an account.
/// </summary>
public interface IAccountCreator {

    /// <summary>
    /// Attempts creating an account and returns the result. 
    /// </summary>
    /// <remarks>
    /// This is a thin <see cref="Task.GetAwaiter"/> wrapper around <see cref="CreateAccountAsync(IReadOnlyAccountModel)"/>.
    /// </remarks>
    /// <param name="model"></param>
    /// <returns></returns>
    CreatorResponseCode CreateAccount(IReadOnlyAccountModel model);

    /// <summary>
    /// Attempts creating an account asynchonously and returns the result.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<CreatorResponseCode> CreateAccountAsync(IReadOnlyAccountModel model);

}
