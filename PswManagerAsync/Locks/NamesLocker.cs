using System.Collections.Concurrent;

namespace PswManagerAsync.Locks {
    internal class NamesLocker : IDisposable {

        readonly ConcurrentDictionary<string, RefCount<SemaphoreSlim>> semaphores = new();
        readonly Locker concurrentLocker = new();
        private bool isDisposed = false;

        public async Task<LockResult> LockAsync(string name, int millisecondsTimeout = -1, string timeoutMessage = "The object is being used elsewhere.") {
            RefCount<SemaphoreSlim> refSemaphore;
            using(var getLock = await concurrentLocker.GetLockAsync()) {
                refSemaphore = GetRefSemaphore(name);
            }
            var entered = await refSemaphore.UseValueAsync(async x => await x.WaitAsync(millisecondsTimeout));
            return LockResult.CreateResult(entered, timeoutMessage);
        }

        public LockResult Lock(string name, int millisecondsTimeout = -1, string timeoutMessage = "The object is being used elsewhere.") {
            RefCount<SemaphoreSlim> refSemaphore;
            using (var getLock = concurrentLocker.GetLock()) {
                refSemaphore = GetRefSemaphore(name);
            }
            var entered = refSemaphore.UseValue(x => x.Wait(millisecondsTimeout));
            return LockResult.CreateResult(entered, timeoutMessage);
        }

        public void Unlock(string name) {
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

    }
}
