using PswManager.Encryption.Services;

namespace PswManager.Core.Services;

/// <summary>
/// Provides methods to check whether the token is set, whether the password is correct, and to set a new token.
/// </summary>
public interface ITokenService {

    /// <summary>
    /// </summary>
    /// <returns>Whether the token is already set.</returns>
    bool IsSet();

    /// <summary>
    /// Sets a new token or overwrites an existing one.
    /// </summary>
    /// <param name="cryptoService"></param>
    void SetToken(ICryptoService cryptoService);

    /// <summary>
    /// Checks if the given <paramref name="cryptoService"/> returns the correct token.
    /// </summary>
    /// <param name="cryptoService"></param>
    /// <returns></returns>
    TokenResult VerifyToken(ICryptoService cryptoService);

    /// <summary>
    /// The results that can arise from checking the token.
    /// </summary>
    public enum TokenResult {
        Unknown,
        Success,
        Failure,
        Missing
    }
}