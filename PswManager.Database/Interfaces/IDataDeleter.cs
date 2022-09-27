using PswManager.Database.DataAccess.ErrorCodes;
using System.Threading.Tasks;

namespace PswManager.Database.Interfaces;
public interface IDataDeleter : IDataHelper
{

    Task<DeleterResponseCode> DeleteAccountAsync(string name);

}
