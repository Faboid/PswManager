using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Inner.Interfaces;
public interface IAccountDeleter {

    Option<DeleterErrorCode> DeleteAccount(string name);
    Task<Option<DeleterErrorCode>> DeleteAccountAsync(string name);

}
