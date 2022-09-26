using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner.Interfaces;
public interface IAccountDeleter {

    Option<DeleterErrorCode> DeleteAccount(string name);
    Task<Option<DeleterErrorCode>> DeleteAccountAsync(string name);

}
