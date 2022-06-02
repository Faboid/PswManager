using PswManager.Database.Models;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountEditor {

        Result UpdateAccount(string name, AccountModel newValues);
        Task<Result> UpdateAccountAsync(string name, AccountModel newValues);

    }
}
