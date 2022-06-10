using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountCreator {

        Option<CreatorErrorCode> CreateAccount(AccountModel model);
        Task<Option<CreatorErrorCode>> CreateAccountAsync(AccountModel model);

    }
}
