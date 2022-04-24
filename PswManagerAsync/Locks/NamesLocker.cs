using System.Collections.Concurrent;

namespace PswManagerAsync.Locks {
    internal class NamesLocker : IDisposable {

        //todo -1 swap usage of SemaphoreSlim with new Locker
        //todo -2 return disposable objects to use the "using" keyword
        readonly ConcurrentDictionary<string, RefCount<SemaphoreSlim>> semaphores = new();
        readonly Locker concurrentLocker = new();
        private bool isDisposed = false;
        
        public async Task<Lock> GetLockAsync(string name, int millisecondsTimeout = -1) {
            RefCount<SemaphoreSlim> refSemaphore;
            using(var getLock = await concurrentLocker.GetLockAsync()) {
                refSemaphore = GetRefSemaphore(name);
            }
            var entered = await refSemaphore.UseValueAsync(async x => await x.WaitAsync(millisecondsTimeout));
            return new(entered, name, this);
        }

        public Lock GetLock(string name, int millisecondsTimeout = -1) {
            RefCount<SemaphoreSlim> refSemaphore;
            using (var getLock = concurrentLocker.GetLock()) {
                refSemaphore = GetRefSemaphore(name);
            }
            var entered = refSemaphore.UseValue(x => x.Wait(millisecondsTimeout));
            return new(entered, name, this);
        }

        private void Unlock(string name) {
            using var getLock = concurrentLocker.GetLock();
            lock(semaphores) {

                if(!semaphores.TryGetValue(name, out var refSemaphore)) {
                    //if there's none, return
                    return;
                }

                lock(refSemaphore) {

                    //if it's not being used, dispose it
                    if(!refSemaphore.IsInUse) {
                        semaphores.TryRemove(name, out _);
                        refSemaphore.UseValue(x => {
                            x.Dispose();
                        });

                        return;
                    }

                    //if it's being used, simply release it
                    refSemaphore.UseValue(x => {
                        x.Release();
                    });

                }

            }
        }

        private RefCount<SemaphoreSlim> GetRefSemaphore(string name) {
            if(isDisposed) {
                throw new ObjectDisposedException($"The {nameof(NamesLocker)} object has been already disposed of.");
            }

            SemaphoreSlim slim = new(1, 1);
            RefCount<SemaphoreSlim> refSlim = new(slim);
            var refSemaphore = semaphores.GetOrAdd(name, refSlim);

            //if the dictionary returns an existing value,
            //the newly-created semaphoreslim is redundant and must be disposed of
            if(!ReferenceEquals(refSlim, refSemaphore)) {
                slim.Dispose();
            }

            return refSemaphore;
        }

        public void Dispose() {
            using(var getLock = concurrentLocker.GetLock()) {
                isDisposed = true;
                Parallel.ForEach(semaphores, refVal => refVal.Value.UseValue(x => x.Dispose()));
                semaphores.Clear();
            }
            concurrentLocker.Dispose();   
        }

        public struct Lock : IDisposable {

            private readonly NamesLocker locker;
            private bool isDisposed = false;

            public readonly bool Obtained { get; init; }
            public readonly string Name { get; init; }

            internal Lock(bool obtained, string name, NamesLocker locker) {
                this.locker = locker;
                Name = name;
                Obtained = obtained;
            }

            public void Dispose() {
                if(isDisposed) {
                    throw new ObjectDisposedException(nameof(GetLock));
                }

                isDisposed = true;
                if(Obtained) {
                    locker.Unlock(Name);
                }
            }
        }

    }
}
