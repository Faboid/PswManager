namespace PswManager.Async.Locks;

/// <summary>
/// A wrapper of <see cref="SemaphoreSlim"/> to simplify waiting & releasing a lock.
/// </summary>
public class Locker : IDisposable {

    /// <summary>
    /// Initializes a <see cref="Locker"/> that allows a single concurrent entry.
    /// </summary>
    public Locker() : this(1) { }

    /// <summary>
    /// Initializes a <see cref="Locker"/> that allows <paramref name="concurrentEntry"/> concurrent entries.
    /// </summary>
    /// <param name="concurrentEntry"></param>
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

    /// <summary>
    /// Blocks the current thread until it can enter <see cref="Locker"/>.
    /// </summary>
    /// <returns></returns>
    public Lock GetLock() {
        semaphore.Wait();
        ThrowIfDisposed();
        return new(true, this);
    }

    /// <summary>
    /// Blocks the current thread until it can enter <see cref="Locker"/> or until <paramref name="millisecondsTimeout"/> times out.
    /// <br/>Check <see cref="Lock.Obtained"/> to see if it has been obtained successfully.
    /// </summary>
    /// <param name="millisecondsTimeout"></param>
    /// <returns></returns>
    public Lock GetLock(int millisecondsTimeout) {
        bool result = semaphore.Wait(millisecondsTimeout);

        ThrowIfDisposed();
        return new(result, this);
    }

    /// <summary>
    /// Blocks the current thread until it can enter <see cref="Locker"/> or until <paramref name="millisecondsTimeout"/> times out, while observing a <see cref="CancellationToken"/>
    /// <br/>Check <see cref="Lock.Obtained"/> to see if it has been obtained successfully.
    /// </summary>
    /// <param name="millisecondsTimeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Lock GetLock(int millisecondsTimeout, CancellationToken cancellationToken) {
        bool result = semaphore.Wait(millisecondsTimeout, cancellationToken);

        ThrowIfDisposed();
        return new(result, this);
    }

    /// <summary>
    /// Asynchronously waits until it can enter <see cref="Locker"/>.
    /// </summary>
    /// <returns></returns>
    public async Task<Lock> GetLockAsync() {
        await semaphore.WaitAsync().ConfigureAwait(false);
        ThrowIfDisposed();
        return new(true, this);
    }

    /// <summary>
    /// Asynchronously waits until it can enter <see cref="Locker"/> or until <paramref name="millisecondsTimeout"/> times out.
    /// <br/>Check <see cref="Lock.Obtained"/> to see if it has been obtained successfully.
    /// </summary>
    /// <param name="millisecondsTimeout"></param>
    /// <returns></returns>
    public async Task<Lock> GetLockAsync(int millisecondsTimeout) {
        bool result = await semaphore.WaitAsync(millisecondsTimeout).ConfigureAwait(false);
        ThrowIfDisposed();
        return new(result, this);
    }

    /// <summary>
    /// Asynchronously waits until it can enter <see cref="Locker"/> while observing a <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Lock> GetLockAsync(CancellationToken cancellationToken) {
        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        ThrowIfDisposed();
        return new(true, this);
    }

    /// <summary>
    /// Asynchronously waits until it can enter <see cref="Locker"/> or until <paramref name="millisecondsTimeout"/> times out, while observing a <see cref="CancellationToken"/>
    /// <br/>Check <see cref="Lock.Obtained"/> to see if it has been obtained successfully.
    /// </summary>
    /// <param name="millisecondsTimeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Lock> GetLockAsync(int millisecondsTimeout, CancellationToken cancellationToken) {
        bool result = await semaphore.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false);
        ThrowIfDisposed();
        return new(result, this);
    }

    private void Unlock() {

        //even if it's disposed, Unlock() should be a valid call
        if(isDisposed) {
            return;
        }

        semaphore.Release();
    }

    private void ReleaseAll() {
        while(semaphore.CurrentCount < 1) {
            semaphore.Release();
        }
    }

    public void Dispose() {
        lock(semaphore) {
            if(isDisposed) {
                return;
            }

            isDisposed = true;
            ReleaseAll();
            semaphore.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// A struct that handles releasing the lock once it's disposed.
    /// </summary>
    public struct Lock : IDisposable {

        private readonly Locker locker;
        private bool isDisposed = false;

        /// <summary>
        /// Whether the lock has been acquired successfully.
        /// </summary>
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

