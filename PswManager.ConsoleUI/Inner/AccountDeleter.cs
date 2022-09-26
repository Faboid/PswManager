using PswManager.ConsoleUI.Inner.Interfaces;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner;
public class AccountDeleter : IAccountDeleter {

    private readonly IDataDeleter dataDeleter;

    public AccountDeleter(IDataDeleter dataDeleter) {
        this.dataDeleter = dataDeleter;
    }

    public Option<DeleterErrorCode> DeleteAccount(string name) {
        return DeleteAccountAsync(name).GetAwaiter().GetResult();
    }

    public async Task<Option<DeleterErrorCode>> DeleteAccountAsync(string name) {

        if(string.IsNullOrWhiteSpace(name)) {
            return DeleterErrorCode.InvalidName;
        }

        return await dataDeleter.DeleteAccountAsync(name).ConfigureAwait(false);
    }

}
