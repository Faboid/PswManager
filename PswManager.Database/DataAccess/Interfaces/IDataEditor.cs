using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces; 

public interface IDataEditor : IDataHelper {

    Task<EditorResponseCode> UpdateAccountAsync(string name, AccountModel newModel);

}
