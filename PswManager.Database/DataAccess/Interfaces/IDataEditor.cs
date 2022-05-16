using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces {
    public interface IDataEditor : IDataHelper {

        ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel);

        ValueTask<ConnectionResult<AccountModel>> UpdateAccountAsync(string name, AccountModel newModel);

    }
}
