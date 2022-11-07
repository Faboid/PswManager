using PswManager.Core.AccountModels;
using System.Threading.Tasks;

namespace PswManager.Core.MasterKey;

internal interface IAccountsHandler {
    Task<IAccountsHandlerExecutable> SetupAccounts(IAccountModelFactory newFactory);
    void UpdateModelFactory(IAccountModelFactory newModelFactory);
}

internal interface IAccountsHandlerExecutable {
    Task ExecuteUpdate();
}