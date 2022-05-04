using PswManagerDatabase.Models;
using System.Collections.Generic;

namespace PswManagerDatabase.DataAccess {
    public static class CachedResults {

        public static ConnectionResult<AccountModel> InvalidNameResult { get; } = new(false, "The given name is not valid.");
        public static ConnectionResult<AccountModel> UsedElsewhereResult { get; } = new(false, "This account is being used elsewhere.");
        public static ConnectionResult<IEnumerable<AccountResult>> SomeAccountUsedElsewhereResult { get; } = new(false, "Some account is being used elsewhere in a long operation.");
        public static ConnectionResult<IAsyncEnumerable<AccountResult>> SomeAccountUsedElsewhereResultAsync { get; } = new(false, "Some account is being used elsewhere in a long operation.");
        public static ConnectionResult<AccountModel> DoesNotExistResult { get; } = new(false, "The given account does not exist.");
        public static ConnectionResult<AccountModel> CreateAccountAlreadyExistsResult { get; } = new(false, "The given account name is already occupied.");
        public static ConnectionResult<AccountModel> NewAccountNameExistsAlreadyResult { get; } = new(false, "There is already an account with that name.");

    }
}
