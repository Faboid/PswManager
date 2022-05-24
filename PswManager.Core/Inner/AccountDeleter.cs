using PswManager.Core.Inner.Interfaces;
using PswManager.Core.UIConnection;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Utils.WrappingObjects;
using System.Threading.Tasks;

namespace PswManager.Core.Inner {
    public class AccountDeleter : IAccountDeleter {

        private readonly IDataDeleter dataDeleter;
        private readonly IUserInput userInput;

        private readonly Result nullOrWhiteSpaceNameResult = new("The name must be assigned.");
        private readonly Result stoppedEarlyResult = new("The operation has been stopped.");
        private readonly Result successResult = new(true);

        public AccountDeleter(IDataDeleter dataDeleter, IUserInput userInput) {
            this.dataDeleter = dataDeleter;
            this.userInput = userInput;
        }

        public Result DeleteAccount(string name) {

            if(string.IsNullOrWhiteSpace(name)) {
                return nullOrWhiteSpaceNameResult;
            }

            if(StopEarlyQuestion()) { return stoppedEarlyResult; }

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

            if(StopEarlyQuestion()) { return stoppedEarlyResult; }

            var cnnResult = await dataDeleter.DeleteAccountAsync(name).ConfigureAwait(false);

            return cnnResult.Success switch {
                true => successResult,
                false => new($"There has been an error: {cnnResult.ErrorMessage}")
            };
        }

        private bool StopEarlyQuestion() {
            //if the user inputs yes, they are sure and want to keep going
            //if the user inputs no, they want to stop
            //as this methods checks if they want to stop, it must be inverted.
            return !userInput.YesOrNo("Are you sure? This account will be deleted forever.");
        }
    }
}
