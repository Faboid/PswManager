using PswManagerCommands.Validation;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Storage;

namespace PswManagerLibrary.Extensions {
    public static class IValidationCollectionExtensions {

        //todo - turn this into a constant value. Consider moving it to ValidationCollection
        public static string InexistentAccountMessage(this IValidationCollection collection) => "The given account doesn't exist.";

        /// <summary>
        /// Adds a condition to make sure the account exists.
        /// </summary>
        /// <param name="dataHelper"></param>
        public static void AddAccountShouldExistCondition(this IValidationCollection collection, IDataHelper dataHelper) {
            collection.Add((args) => dataHelper.AccountExist(args[0]) == true, collection.InexistentAccountMessage());
        }

    }
}
