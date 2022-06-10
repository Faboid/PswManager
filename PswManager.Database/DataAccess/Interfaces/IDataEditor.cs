using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces; 

public interface IDataEditor : IDataHelper {

    Option<EditorErrorCode> UpdateAccount(string name, AccountModel newModel);
    ValueTask<Option<EditorErrorCode>> UpdateAccountAsync(string name, AccountModel newModel);

}
