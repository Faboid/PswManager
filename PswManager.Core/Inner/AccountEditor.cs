using PswManager.Core.Inner.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils.WrappingObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManager.Core.Inner {
    public class AccountEditor : IAccountEditor {
        public Result<AccountModel> UpdateAccount(string name, AccountModel newValues) {
            throw new NotImplementedException();
        }

        public Task<Result<AccountModel>> UpdateAccountAsync(string name, AccountModel newValues) {
            throw new NotImplementedException();
        }
    }
}
