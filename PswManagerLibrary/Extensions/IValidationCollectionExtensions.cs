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
        /// <param name="pswManager"></param>
        public static void AddAccountShouldExistCondition(this IValidationCollection collection, ushort index, IDataHelper dataHelper) {
            collection.Add(new IndexHelper(index, collection.NullIndexCondition, collection.NullOrEmptyArgsIndexCondition, collection.CorrectArgsNumberIndexCondition), 
                (args) => dataHelper.AccountExist(args[0]) == true, collection.InexistentAccountMessage());
        }

    }
}
