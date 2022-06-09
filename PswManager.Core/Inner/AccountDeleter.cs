using PswManager.Core.Inner.Interfaces;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Inner {
    public class AccountDeleter : IAccountDeleter {

        private readonly IDataDeleter dataDeleter;

        private readonly Result nullOrWhiteSpaceNameResult = new("The name must be assigned.");
        private readonly Result successResult = new(true);

        public AccountDeleter(IDataDeleter dataDeleter) {
            this.dataDeleter = dataDeleter;
        }

        //todo - consider removing name validation logic
        //why? because it's hindering readability.
        //the name is checked already by the repo's code,
        //and the returning result is pretty much the same

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
}
