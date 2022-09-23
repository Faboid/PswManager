using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Inner.Interfaces; 
public interface IAccountEditor {

    Option<EditorErrorCode> UpdateAccount(string name, AccountModel newValues);
    Task<Option<EditorErrorCode>> UpdateAccountAsync(string name, AccountModel newValues);

}
