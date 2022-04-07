using PswManagerEncryption.Cryptography;
using PswManagerEncryption.Random;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("PswManagerTests")]
namespace PswManagerEncryption.Services {
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

        private readonly Rfc2898DeriveBytes rfc;
        private readonly SaltRandom random;
        private readonly Channel<Key> channel;
        private readonly Task generationTask;

        private async Task AutoGenerateKeysAsync() {
            while(!channel.Token.IsCancellationRequested) {
                try {
                    var key = await GenerateNextKeyAsync(channel.Token);
                    await channel.WriteAsync(key);
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
                keys.Add(await GenerateKeyAsync());
            }

            return keys;
        }

        public async Task<Key> GenerateKeyAsync() {

            return await channel.ReadAsync();
        }

        private async Task<Key> GenerateNextKeyAsync(CancellationToken cancellationToken) {

            return await Task.Run(() => {
                return GenerateNextKey();
            }, cancellationToken);

        }

        private Key GenerateNextKey() {
            lock(rfc) {
                lock(random) {
                    return new(rfc.GetBytes(random.Next()));
                }
            }
        }

        public async ValueTask DisposeAsync() {
            channel.Dispose();
            await generationTask; //this is to find any exception thrown in there
            rfc.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    internal class Channel<T> : IDisposable {

        public Channel(int length) {
            buffer = new();
            cts = new();
            Token = cts.Token;
            readSemaphore = new(0);
            writeSemaphore = new(length);
        }

        private readonly ConcurrentQueue<T> buffer;
        private readonly SemaphoreSlim readSemaphore;
        private readonly SemaphoreSlim writeSemaphore;
        private readonly CancellationTokenSource cts;

        public CancellationToken Token { get; }

        //public async Task<bool> TryReadAsync(int milliseconds, out T? value) {
        //    value = default;
        //    bool entered = await readSemaphore.WaitAsync(milliseconds, Token);
        //    if(!entered) {
        //        return false;
        //    }
        //    bool success = buffer.TryDequeue(out value);
        //    return success;
        //}

        public async Task<T> ReadAsync() {

            T? output;
            bool success;
            do {
                //if readSemaphore enters but there's no value, it was erranously released
                //therefore, it's fine to lock it in "excess"
                await readSemaphore.WaitAsync(Token);
                success = buffer.TryDequeue(out output);
            } while(!success);

            writeSemaphore.Release();
            return output!;
        }

        public async Task WriteAsync(T value) {
            await writeSemaphore.WaitAsync(Token);
            buffer.Enqueue(value);
            readSemaphore.Release();
        }

        public void Dispose() {
            cts.Cancel();
            readSemaphore.Dispose();
            writeSemaphore.Dispose();
            cts.Dispose();
            buffer.Clear();
        }
    }
}
