using PswManager.ConsoleUI.Inner.Interfaces;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner;
public class AccountDeleter : IAccountDeleter {

    private readonly IDataDeleter dataDeleter;

    public AccountDeleter(IDataDeleter dataDeleter) {
        this.dataDeleter = dataDeleter;
    }

    public DeleterResponseCode DeleteAccount(string name) {
        return DeleteAccountAsync(name).GetAwaiter().GetResult();
    }

    public async Task<DeleterResponseCode> DeleteAccountAsync(string name) {

        if(string.IsNullOrWhiteSpace(name)) {
            return DeleterResponseCode.InvalidName;
        }

        return await dataDeleter.DeleteAccountAsync(name).ConfigureAwait(false);
    }

}
