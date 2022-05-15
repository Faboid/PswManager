using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces {
    public interface IDataCreator : IDataHelper {

        ConnectionResult CreateAccount(AccountModel model);
        Task<ConnectionResult> CreateAccountAsync(AccountModel model);

    }
}
