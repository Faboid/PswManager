using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManager.Core.Inner {
    internal static class AccountExtensions {

        static readonly Result nullOrWhiteSpaceNameResult = new("You must provide a valid account name.");
        static readonly Result nullOrWhiteSpacePasswordResult = new("You must provide a valid password.");
        static readonly Result nullOrWhiteSpaceEmailResult = new("You must provide a valid email.");

        public static bool IsAnyValueNullOrEmpty(this AccountModel account, out Result failureResult) {

            if(string.IsNullOrWhiteSpace(account.Name)) {
                failureResult = nullOrWhiteSpaceNameResult;
                return true;
            }

            if(string.IsNullOrWhiteSpace(account.Password)) {
                failureResult = nullOrWhiteSpacePasswordResult;
                return true;
            }

            if(string.IsNullOrWhiteSpace(account.Email)) {
                failureResult = nullOrWhiteSpaceEmailResult;
                return true;
            }

            failureResult = null;
            return false;

        }

    }
}
