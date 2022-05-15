using PswManagerDatabase.Models;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataEditor : IDataHelper {

        ConnectionResult<AccountModel> UpdateAccount(string name, AccountModel newModel);

        ValueTask<ConnectionResult<AccountModel>> UpdateAccountAsync(string name, AccountModel newModel);

    }
}
