using System.Threading.Tasks;

namespace PswManager.Core.MasterKey;
public interface IPasswordEditor {
    Task<PasswordChangeResult> ChangePasswordTo(string password);
}