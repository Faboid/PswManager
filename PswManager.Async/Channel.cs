using PswManager.Async.Locks;
using PswManager.Extensions;
using System.Collections.Concurrent;

namespace PswManager.Async {

    public class Channel<T> : IDisposable {

        public Channel(int length) {
            buffer = new();
            cts = new();
            Token = cts.Token;

            //creates new locker, then takes up all waiters
            readLocker = new(length);
            readLocks = new();
            Enumerable.Range(0, length)
               .Select(x => readLocker.GetLock())
               .ForEach(x => readLocks.Enqueue(x));

            //creates new locker and leaves all locks free
            writeLocker = new(length);
            writeLocks = new();
        }

        private readonly ConcurrentQueue<T> buffer;
        
        private readonly Locker readLocker;
        private readonly Locker writeLocker;

        private readonly ConcurrentQueue<Locker.Lock> readLocks;
        private readonly ConcurrentQueue<Locker.Lock> writeLocks;

        private readonly CancellationTokenSource cts;
        private bool isDisposed = false;

        public CancellationToken Token { get; }

        public async Task<(bool success, T? value)> TryReadAsync(int milliseconds) {
            var lockOwned = await readLocker.GetLockAsync(milliseconds, Token).ConfigureAwait(false);
            if(!lockOwned.Obtained) {
                return (false, default);
            }
            
            bool success = buffer.TryDequeue(out T? value);
            
            //get write lock and free it, so writing threads can write
            writeLocks.TryDequeue(out var writeLock);
            writeLock.Dispose();

            if(!success) {
                //if the dequeue has failed, return everything to its previous state.
                lockOwned.Dispose();
                return (false, default);
            }

            readLocks.Enqueue(lockOwned);
            return (success, value);
        }

        public async Task<T> ReadAsync() {

            T? output = default;
            bool success = false;
            do {
                var readLock = await readLocker.GetLockAsync(5000, Token).ConfigureAwait(false);
                if(!readLock.Obtained) {
                    continue;
                }
                readLocks.Enqueue(readLock);

                //if readLocker enters but there's no value, it was erranously released
                //therefore, it's fine to lock it in "excess"
                success = buffer.TryDequeue(out output);
            } while(!success || output == null);

            //get write lock and free it, so writing threads can write
            writeLocks.TryDequeue(out var writeLock);
            writeLock.Dispose();

            return output!;
        }

        public async Task WriteAsync(T value) {
            var writeLock = await writeLocker.GetLockAsync().ConfigureAwait(false);
            writeLocks.Enqueue(writeLock);
            buffer.Enqueue(value);
            readLocks.TryDequeue(out var readLock);
            readLock.Dispose();
        }

        public void Dispose() {
            lock(cts) {
                if(isDisposed) {
                    return;
                }

                if(!cts.IsCancellationRequested) {
                    cts.Cancel();
                }
                readLocker.Dispose();
                writeLocker.Dispose();
                cts.Dispose();
                buffer.Clear();
                GC.SuppressFinalize(this);

                isDisposed = true;
            }

        }
    }
}
