using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner.Interfaces;
public interface IAccountEditor {

    EditorResponseCode UpdateAccount(string name, AccountModel newValues);
    Task<EditorResponseCode> UpdateAccountAsync(string name, AccountModel newValues);

}
