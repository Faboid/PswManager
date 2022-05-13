namespace PswManagerAsync.Locks {
    public class Locker : IDisposable {

        public Locker() : this(1) { }

        public Locker(int concurrentEntry) {
            semaphore = new(concurrentEntry, concurrentEntry);
        }

        private readonly SemaphoreSlim semaphore;

        public Lock GetLock() {
            semaphore.Wait();
            return new(true, this);
        }

        public Lock GetLock(int millisecondsTimeout) {
            return new(semaphore.Wait(millisecondsTimeout), this);
        }

        public Lock GetLock(int millisecondsTimeout, CancellationToken cancellationToken) {
            return new(semaphore.Wait(millisecondsTimeout, cancellationToken), this);
        }

        public async Task<Lock> GetLockAsync() {
            await semaphore.WaitAsync().ConfigureAwait(false);
            return new(true, this);
        }

        public async Task<Lock> GetLockAsync(int millisecondsTimeout) {
            return new(await semaphore.WaitAsync(millisecondsTimeout).ConfigureAwait(false), this);
        }

        public async Task<Lock> GetLockAsync(CancellationToken cancellationToken) {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            return new(true, this);
        }

        public async Task<Lock> GetLockAsync(int millisecondsTimeout, CancellationToken cancellationToken) {
            return new(await semaphore.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false), this);
        }

        private void Unlock() {
            semaphore.Release();
        }

        public void Dispose() {
            semaphore.Dispose();
            GC.SuppressFinalize(this);
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
