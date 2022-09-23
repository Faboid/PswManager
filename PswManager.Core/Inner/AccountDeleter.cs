using PswManager.Core.Inner.Interfaces;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Inner; 
public class AccountDeleter : IAccountDeleter {

    private readonly IDataDeleter dataDeleter;

    public AccountDeleter(IDataDeleter dataDeleter) {
        this.dataDeleter = dataDeleter;
    }

    public Option<DeleterErrorCode> DeleteAccount(string name) {

        if(string.IsNullOrWhiteSpace(name)) {
            return DeleterErrorCode.InvalidName;
        }

        return dataDeleter.DeleteAccount(name);
    }

    public async Task<Option<DeleterErrorCode>> DeleteAccountAsync(string name) {

        if(string.IsNullOrWhiteSpace(name)) {
            return DeleterErrorCode.InvalidName;
        }

        return await dataDeleter.DeleteAccountAsync(name).ConfigureAwait(false);
    }

}
