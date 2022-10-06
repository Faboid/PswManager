using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.UI.Console.Inner.Interfaces;

/// <summary>
/// Provides methods to edit accounts.
/// </summary>
public interface IAccountEditor {

    /// <summary>
    /// Tries to edit an account and returns the result. 
    /// </summary>
    /// <remarks>
    /// This is a thin <see cref="Task.Wait"/> wrapper around <see cref="UpdateAccountAsync(string, IReadOnlyAccountModel)"/>.
    /// </remarks>
    /// <param name="name"></param>
    /// <param name="newValues"></param>
    /// <returns></returns>
    EditorResponseCode UpdateAccount(string name, IReadOnlyAccountModel newValues);

    /// <summary>
    /// Tries to edit an account asynchronously and returns the result.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="newValues"></param>
    /// <returns></returns>
    Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newValues);

}
