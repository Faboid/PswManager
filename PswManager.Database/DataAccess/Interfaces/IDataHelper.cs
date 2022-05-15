using System.Threading.Tasks;

namespace PswManager.Database.DataAccess.Interfaces {
    public interface IDataHelper {
        //todo - decide whether to turn these into connection results

        bool AccountExist(string name);

        ValueTask<bool> AccountExistAsync(string name);

    }
}
