using PswManager.Encryption.Services;

namespace PswManager.Core.Services;
public interface ITokenService {
    bool IsSet();
    void SetToken(ICryptoService cryptoService);
    TokenResult VerifyToken(ICryptoService cryptoService);

    public enum TokenResult {
        Unknown,
        Success,
        Failure,
        Missing
    }
}