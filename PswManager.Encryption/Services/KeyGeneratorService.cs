using PswManager.Encryption.Cryptography;
using PswManager.Encryption.Random;
using PswManager.Async;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using PswManager.Async.Locks;

[assembly: InternalsVisibleTo("PswManagerTests")]
namespace PswManager.Encryption.Services {
    public class KeyGeneratorService : IAsyncDisposable {

        /// <summary>
        /// Instantiates a <see cref="KeyGeneratorService"/> with a fixed salt. 
        /// As long as the given <paramref name="masterKey"/> is the same, the generated keys will be consistent.
        /// </summary>
        /// <param name="masterKey"></param>
        public KeyGeneratorService(char[] masterKey) : this(new byte[] { 64, 12, 35, 44, 21, 34, 43 }, masterKey) { }

        /// <summary>
        /// Instantiates a <see cref="KeyGeneratorService"/> with a custom <paramref name="salt"/>.
        /// As long as <paramref name="salt"/> (and <paramref name="masterKey"/>) is the same, the generated keys will be consistent.
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="masterKey"></param>
        public KeyGeneratorService(byte[] salt, char[] masterKey) : this(salt, masterKey, 1000000){ }

        /// <summary>
        /// Instantiates a <see cref="KeyGeneratorService"/> with a custom <paramref name="salt"/>.
        /// As long as <paramref name="salt"/>, <paramref name="masterKey"/>, and <paramref name="iterations"/> are the same, the generated keys will be consistent.
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="masterKey"></param>
        /// <param name="iterations"></param>
        internal KeyGeneratorService(byte[] salt, char[] masterKey, int iterations) {
            using var key = new Key(masterKey);
            byte[] bytes = Encoding.Unicode.GetBytes(key.Get());

            rfc = new Rfc2898DeriveBytes(bytes, salt, iterations);
            random = new SaltRandom(64, 168, bytes.Sum(x => x / (bytes.Length / 2)));

            channel = new(3);
            generationTask = AutoGenerateKeysAsync();
        }

        public bool IsDisposed { get; private set; } = false;

        private readonly Rfc2898DeriveBytes rfc;
        private readonly SaltRandom random;
        private readonly Channel<Key> channel;
        private readonly Task generationTask;
        private readonly Locker locker = new(1);

        private async Task AutoGenerateKeysAsync() {
            while(!channel.Token.IsCancellationRequested && !IsDisposed) {
                try {
                    var key = await GenerateNextKeyAsync(channel.Token).ConfigureAwait(false);
                    await channel.WriteAsync(key).ConfigureAwait(false);
                } catch (OperationCanceledException) {
                    //do nothing
                    return;
                } catch (ObjectDisposedException) {
                    //still do nothing
                    return;
                }
            }
        }

        /// <summary>
        /// Generates a finite amount of keys.
        /// <br/>As long as the given <see cref="salt"/> and <see cref="masterKey"/> are the same, 
        /// the generated series will be the same across instances.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public async Task<List<Key>> GenerateKeysAsync(int num) {

            List<Key> keys = new();

            while(keys.Count < num) {
                keys.Add(await GenerateKeyAsync().ConfigureAwait(false));
            }

            return keys;
        }

        public async Task<Key> GenerateKeyAsync() {
            using var lockhere = await locker.GetLockAsync().ConfigureAwait(false);
            do {

                //check generationTask to make sure it hasn't thrown an exception
                if(generationTask.IsFaulted) {
                    try {
                        await generationTask.ConfigureAwait(false);
                    }
                    catch(Exception ex) {
                        //todo - insert some kind of logging here
                        throw new Exception("The infinite key-generation task has thrown an exception.", ex);
                    }
                }

                if(IsDisposed) {
                    throw new ObjectDisposedException("This object has already been disposed of.");
                }

                var (success, key) = await channel.TryReadAsync(2000).ConfigureAwait(false);
                if(success) {
                    return key!;
                }

            } while(true);
        }

        private async Task<Key> GenerateNextKeyAsync(CancellationToken cancellationToken) {
            return await Task.Run(() => {
                return GenerateNextKey();
            }, cancellationToken).ConfigureAwait(false);

        }

        private Key GenerateNextKey() {
            return new(rfc.GetBytes(random.Next()));
        }

        public async ValueTask DisposeAsync() {
            if(IsDisposed) {
                return;
            }
            
            channel.Dispose();
            await generationTask.ConfigureAwait(false); //this is to find any exception thrown in there
            rfc.Dispose();
            IsDisposed = true;
            locker.Dispose();
            GC.SuppressFinalize(this);

        }
    }
}
