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
