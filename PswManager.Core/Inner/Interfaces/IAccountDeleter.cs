using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Inner.Interfaces {
    public interface IAccountDeleter {

        Result DeleteAccount(string name);
        Task<Result> DeleteAccountAsync(string name);

    }
}
