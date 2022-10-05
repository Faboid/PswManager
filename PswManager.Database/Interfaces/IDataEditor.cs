using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Database.Interfaces;

/// <summary>
/// Provides methods to edit accounts.
/// </summary>
public interface IDataEditor : IDataHelper {

    /// <summary>
    /// Attempts to edit an account asynchonously and returns the result.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="newModel"></param>
    /// <returns></returns>
    Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newModel);

}
