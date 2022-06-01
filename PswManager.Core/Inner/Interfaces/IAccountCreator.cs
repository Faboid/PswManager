using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;
using System.Threading.Tasks;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountCreator {

        Result CreateAccount(AccountModel model);
        Task<Result> CreateAccountAsync(AccountModel model);

    }
}
