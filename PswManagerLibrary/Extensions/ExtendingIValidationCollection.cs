using PswManagerCommands.Validation;
using PswManagerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Extensions {
    public static class ExtendingIValidationCollection {

        public static string InexistentAccountMessage(this IValidationCollection collection) => "The given account doesn't exist.";

        /// <summary>
        /// Adds a condition to make sure the account exists.
        /// </summary>
        /// <param name="pswManager"></param>
        public static void AddAccountShouldExistCondition(this IValidationCollection collection, IPasswordManager pswManager) {
            collection.Add((args) => pswManager.AccountExist(args[0]) == true, collection.InexistentAccountMessage());
        }

    }
}
