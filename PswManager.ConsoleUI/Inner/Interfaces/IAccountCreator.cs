using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner.Interfaces;
public interface IAccountCreator {

    CreatorResponseCode CreateAccount(AccountModel model);
    Task<CreatorResponseCode> CreateAccountAsync(AccountModel model);

}
