using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner.Interfaces;
public interface IAccountDeleter {

    DeleterResponseCode DeleteAccount(string name);
    Task<DeleterResponseCode> DeleteAccountAsync(string name);

}
