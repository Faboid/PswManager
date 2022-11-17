using System.Threading.Tasks;

namespace PswManager.Core.MasterKey;
internal interface IPasswordStatusChecker {
    void Free();
    Task<PasswordStatusChecker.PasswordStatus> GetStatus();
    Task SetStatusTo(PasswordStatusChecker.PasswordStatus status);
}