using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Services;

/// <summary>
/// Provides methods to log in and sign in, which will instantiate a <see cref="ICryptoAccountService"/>.
/// </summary>
public interface ICryptoAccountServiceFactory {

    /// <summary>
    /// Checks whether the given password is correct. Will return <see cref="ICryptoAccountService"/> if it is; else, <see cref="ITokenService.TokenResult"/>.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<Option<ICryptoAccountService, ITokenService.TokenResult>> LogInAccountAsync(char[] password);

    /// <summary>
    /// Creates a token for future use with the given password, then returns a <see cref="ICryptoAccountService"/> build with it.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<ICryptoAccountService> SignUpAccountAsync(char[] password);

    /// <summary>
    /// Builds a <see cref="ICryptoAccountService"/>, but doesn't save it to the token.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    internal Task<ICryptoAccountService> BuildCryptoAccountService(char[] password);

}