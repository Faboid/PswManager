using PswManager.Core.Inner.Interfaces;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Utils.WrappingObjects;
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

        public Result DeleteAccount(string name) {

            if(string.IsNullOrWhiteSpace(name)) {
                return nullOrWhiteSpaceNameResult;
            }

            var cnnResult = dataDeleter.DeleteAccount(name);

            return cnnResult.Success switch {
                true => successResult,
                false => new($"There has been an error: {cnnResult.ErrorMessage}")
            };
        }

        public async Task<Result> DeleteAccountAsync(string name) {

            if(string.IsNullOrWhiteSpace(name)) {
                return nullOrWhiteSpaceNameResult;
            }

            var cnnResult = await dataDeleter.DeleteAccountAsync(name).ConfigureAwait(false);

            return cnnResult.Success switch {
                true => successResult,
                false => new($"There has been an error: {cnnResult.ErrorMessage}")
            };
        }

    }
}
