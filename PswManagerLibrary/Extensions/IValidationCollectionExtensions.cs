using PswManagerCommands.Validation;
using PswManagerLibrary.Storage;

namespace PswManagerLibrary.Extensions {
    public static class IValidationCollectionExtensions {

        //todo - turn this into a constant value. Consider moving it to ValidationCollection
        public static string InexistentAccountMessage(this IValidationCollection collection) => "The given account doesn't exist.";

        /// <summary>
        /// Adds a condition to make sure the account exists.
        /// </summary>
        /// <param name="pswManager"></param>
        public static void AddAccountShouldExistCondition(this IValidationCollection collection, ushort index, IPasswordManager pswManager) {
            collection.Add(new IndexHelper(index), (args) => pswManager.AccountExist(args[0]) == true, collection.InexistentAccountMessage());
        }

    }
}
