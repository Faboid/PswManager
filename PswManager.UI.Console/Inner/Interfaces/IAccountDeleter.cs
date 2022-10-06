using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner.Interfaces;
/// <summary>
/// Provides methods to delete accounts.
/// </summary>
public interface IAccountDeleter {

    /// <summary>
    /// Attempts to delete an account and returns the result. 
    /// </summary>
    /// <remarks>
    /// This is a thin <see cref="Task.GetAwaiter"/> wrapper on <see cref="DeleteAccountAsync(string)"/>.
    /// </remarks>
    /// <param name="name"></param>
    /// <returns></returns>
    DeleterResponseCode DeleteAccount(string name);

    /// <summary>
    /// Attempts to delete an account asynchonously and returns the result.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<DeleterResponseCode> DeleteAccountAsync(string name);

}
