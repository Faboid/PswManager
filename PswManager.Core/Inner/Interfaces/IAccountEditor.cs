using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;
using System.Threading.Tasks;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountEditor {

        Result<AccountModel> UpdateAccount(string name, AccountModel newValues);
        Task<Result<AccountModel>> UpdateAccountAsync(string name, AccountModel newValues);

    }
}
