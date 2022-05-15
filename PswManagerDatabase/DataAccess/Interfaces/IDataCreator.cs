using PswManagerDatabase.Models;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.Interfaces {
    public interface IDataCreator : IDataHelper {

        ConnectionResult CreateAccount(AccountModel model);
        Task<ConnectionResult> CreateAccountAsync(AccountModel model);

    }
}
