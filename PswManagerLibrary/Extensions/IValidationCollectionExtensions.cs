using PswManagerCommands.Validation;
using PswManagerDatabase.DataAccess.Interfaces;
using PswManagerLibrary.Storage;

namespace PswManagerLibrary.Extensions {
    public static class IValidationCollectionExtensions {

        public static string InexistentAccountMessage<T>(this IValidationCollection<T> collection) => "The given account doesn't exist.";

        /// <summary>
        /// Adds a condition to make sure the account exists.
        /// </summary>
        /// <param name="pswManager"></param>
        public static void AddAccountShouldExistCondition<T>(this IValidationCollection<T> collection, ushort index, string name, IDataHelper dataHelper) {
            collection.Add(index, dataHelper.AccountExist(name) == true, collection.InexistentAccountMessage());
        }

    }
}
