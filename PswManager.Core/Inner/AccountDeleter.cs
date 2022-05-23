using PswManager.Core.Inner.Interfaces;
using PswManager.Utils.WrappingObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManager.Core.Inner {
    public class AccountDeleter : IAccountDeleter {
        public Result DeleteAccount(string name) {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteAccountAsync(string name) {
            throw new NotImplementedException();
        }
    }
}
