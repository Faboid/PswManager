using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Database.Interfaces;

public interface IDataEditor : IDataHelper {

    Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newModel);

}
