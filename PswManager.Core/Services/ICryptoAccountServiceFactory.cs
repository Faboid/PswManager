using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Services;
public interface ICryptoAccountServiceFactory {
    Task<Option<ICryptoAccountService, ITokenService.TokenResult>> LogInAccountAsync(char[] password);
    Task<ICryptoAccountService> SignUpAccountAsync(char[] password);
}