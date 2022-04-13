using System.Collections.Concurrent;

namespace PswManagerAsync.Locks {
    internal class NamesLocker {

        readonly ConcurrentDictionary<string, RefCount<SemaphoreSlim>> semaphores = new();
        readonly SemaphoreSlim concurrentSemaphore = new(1, 1);

        public async Task<LockResult> LockAsync(string name, int millisecondsTimeout = -1, string timeoutMessage = "The object is being used elsewhere.") {
            await concurrentSemaphore.WaitAsync();
            RefCount<SemaphoreSlim> refSemaphore;
            try {
                refSemaphore = GetRefSemaphore(name);
            }
            finally {
                concurrentSemaphore.Release();
            }
            var entered = await refSemaphore.UseValueAsync(async x => await x.WaitAsync(millisecondsTimeout));
            return LockResult.CreateResult(entered, timeoutMessage);
        }

        public LockResult Lock(string name, int millisecondsTimeout = -1, string timeoutMessage = "The object is being used elsewhere.") {
            concurrentSemaphore.Wait();
            RefCount<SemaphoreSlim> refSemaphore;
            try {
                refSemaphore = GetRefSemaphore(name);
            }
            finally {
                concurrentSemaphore.Release();
            }
            var entered = refSemaphore.UseValue(x => x.Wait(millisecondsTimeout));
            return LockResult.CreateResult(entered, timeoutMessage);
        }

        public void Unlock(string name) {
            concurrentSemaphore.Wait();
            try {

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
            finally {
                concurrentSemaphore.Release();
            }
        }

        private RefCount<SemaphoreSlim> GetRefSemaphore(string name) {
            SemaphoreSlim slim = new(1, 1);
            RefCount<SemaphoreSlim> refSlim = new(slim);
            var refSemaphore = semaphores.GetOrAdd(name, refSlim);

            if(!ReferenceEquals(refSlim, refSemaphore)) {
                slim.Dispose();
            }

            return refSemaphore;
        }

    }
}
