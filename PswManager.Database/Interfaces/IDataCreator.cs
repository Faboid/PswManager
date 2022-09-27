using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Models;
using System.Threading.Tasks;

namespace PswManager.Database.Interfaces;
public interface IDataCreator : IDataHelper
{

    Task<CreatorResponseCode> CreateAccountAsync(AccountModel model);

}
