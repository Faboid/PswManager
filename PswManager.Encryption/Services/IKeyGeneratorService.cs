using PswManager.Encryption.Cryptography;

namespace PswManager.Encryption.Services;
public interface IKeyGeneratorService : IAsyncDisposable {

    bool IsDisposed { get; }

    Task<Key> GenerateKeyAsync();
    Task<List<Key>> GenerateKeysAsync(int num);

}