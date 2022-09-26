using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner.Interfaces;
public interface IAccountCreator {

    Option<CreatorErrorCode> CreateAccount(AccountModel model);
    Task<Option<CreatorErrorCode>> CreateAccountAsync(AccountModel model);

}
