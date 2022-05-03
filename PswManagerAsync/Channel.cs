using System.Collections.Concurrent;

namespace PswManagerAsync {

    public class Channel<T> : IDisposable {

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

        public async Task<(bool success, T? value)> TryReadAsync(int milliseconds) {
            bool entered = await readSemaphore.WaitAsync(milliseconds, Token).ConfigureAwait(false);
            if(!entered) {
                return (false, default);
            }
            bool success = buffer.TryDequeue(out T? value);
            writeSemaphore.Release();
            return (success, value);
        }

        public async Task<T> ReadAsync() {

            T? output;
            bool success;
            do {
                //if readSemaphore enters but there's no value, it was erranously released
                //therefore, it's fine to lock it in "excess"
                await readSemaphore.WaitAsync(Token).ConfigureAwait(false);
                success = buffer.TryDequeue(out output);
            } while(!success);

            writeSemaphore.Release();
            return output!;
        }

        public async Task WriteAsync(T value) {
            await writeSemaphore.WaitAsync(Token).ConfigureAwait(false);
            buffer.Enqueue(value);
            readSemaphore.Release();
        }

        public void Dispose() {
            cts.Cancel();
            readSemaphore.Dispose();
            writeSemaphore.Dispose();
            cts.Dispose();
            buffer.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
