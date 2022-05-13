namespace PswManagerAsync.Locks {
    public class Locker : IDisposable {

        public Locker() : this(1) { }

        public Locker(int concurrentEntry) {
            semaphore = new(concurrentEntry, concurrentEntry);
        }

        private readonly SemaphoreSlim semaphore;
        private bool isDisposed = false;

        private void ThrowIfDisposed() {
            if(isDisposed) {
                throw new ObjectDisposedException(nameof(Locker));
            }
        }

        public Lock GetLock() {
            semaphore.Wait();
            ThrowIfDisposed();
            return new(true, this);
        }

        public Lock GetLock(int millisecondsTimeout) {
            bool result = semaphore.Wait(millisecondsTimeout);
            
            ThrowIfDisposed();
            return new(result, this);
        }

        public Lock GetLock(int millisecondsTimeout, CancellationToken cancellationToken) {
            bool result = semaphore.Wait(millisecondsTimeout, cancellationToken);

            ThrowIfDisposed();
            return new(result, this);
        }

        public async Task<Lock> GetLockAsync() {
            await semaphore.WaitAsync().ConfigureAwait(false);
            ThrowIfDisposed();
            return new(true, this);
        }

        public async Task<Lock> GetLockAsync(int millisecondsTimeout) {
            bool result = await semaphore.WaitAsync(millisecondsTimeout).ConfigureAwait(false);
            ThrowIfDisposed();
            return new(result, this);
        }

        public async Task<Lock> GetLockAsync(CancellationToken cancellationToken) {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            ThrowIfDisposed();
            return new(true, this);
        }

        public async Task<Lock> GetLockAsync(int millisecondsTimeout, CancellationToken cancellationToken) {
            bool result = await semaphore.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false);
            ThrowIfDisposed();
            return new(result, this);
        }

        private void Unlock() {
            semaphore.Release();
        }

        private void ReleaseAll() {
            isDisposed = true;
            while(semaphore.CurrentCount < 1) {
                semaphore.Release();
            }
        }

        public void Dispose() {
            lock(semaphore) {
                ReleaseAll();
                semaphore.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        public struct Lock : IDisposable {

            private readonly Locker locker;
            private bool isDisposed = false;

            public readonly bool Obtained { get; init; }

            internal Lock(bool obtained, Locker locker) {
                this.locker = locker;
                Obtained = obtained;
            }

            public void Dispose() {
                if(isDisposed) {
                    throw new ObjectDisposedException(nameof(Lock));
                }

                isDisposed = true;
                if(Obtained) {
                    locker.Unlock();
                }
            }
        }

    }

}
