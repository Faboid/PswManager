using System.Collections.Concurrent;

namespace PswManagerAsync.Locks {
    internal class NamesLocker : IDisposable {

        readonly ConcurrentDictionary<string, RefCount<Locker>> lockers = new();
        readonly Locker concurrentLocker = new();
        private bool isDisposed = false;
        
        public async Task<Lock> GetLockAsync(string name, int millisecondsTimeout = -1) {
            RefCount<Locker> refLocker;
            using(var getLock = await concurrentLocker.GetLockAsync()) {
                refLocker = GetRefLocker(name);
            }
            var heldLock = await refLocker.UseValueAsync(async x => await x.GetLockAsync(millisecondsTimeout));
            return new(name, heldLock, this);
        }

        public Lock GetLock(string name, int millisecondsTimeout = -1) {
            RefCount<Locker> refLocker;
            using (var getLock = concurrentLocker.GetLock()) {
                refLocker = GetRefLocker(name);
            }
            var heldLock = refLocker.UseValue(x => x.GetLock(millisecondsTimeout));
            return new(name, heldLock, this);
        }

        private void Unlock(string name, Locker.Lock internalLock) {
            using var getLock = concurrentLocker.GetLock();
            lock(lockers) {

                if(!lockers.TryGetValue(name, out var refLocker)) {
                    //if there's none, return
                    return;
                }

                lock(refLocker) {

                    //if it's not being used, dispose it
                    if(!refLocker.IsInUse) {
                        lockers.TryRemove(name, out _);
                        refLocker.UseValue(x => {
                            x.Dispose();
                        });

                        return;
                    }

                    //if it's being used, simply release it
                    internalLock.Dispose();

                }

            }
        }

        private RefCount<Locker> GetRefLocker(string name) {
            if(isDisposed) {
                throw new ObjectDisposedException($"The {nameof(NamesLocker)} object has been already disposed of.");
            }

            Locker defaultLocker = new(1);
            RefCount<Locker> refSlim = new(defaultLocker);
            var refLocker = lockers.GetOrAdd(name, refSlim);

            //if the dictionary returns an existing value,
            //the newly-created locker is redundant and must be disposed of
            if(!ReferenceEquals(refSlim, refLocker)) {
                defaultLocker.Dispose();
            }

            return refLocker;
        }

        public void Dispose() {
            using(var getLock = concurrentLocker.GetLock()) {
                isDisposed = true;
                Parallel.ForEach(lockers, refVal => refVal.Value.UseValue(x => x.Dispose()));
                lockers.Clear();
            }
            concurrentLocker.Dispose();   
        }

        public struct Lock : IDisposable {

            private readonly NamesLocker locker;
            private readonly Locker.Lock internalLock;
            private bool isDisposed = false;

            public readonly bool Obtained { get; init; }
            public readonly string Name { get; init; }

            internal Lock(string name, Locker.Lock internalLock, NamesLocker locker) {
                Name = name;
                this.internalLock = internalLock;
                this.locker = locker;
                Obtained = internalLock.Obtained;
            }

            public void Dispose() {
                if(isDisposed) {
                    throw new ObjectDisposedException(nameof(GetLock));
                }

                isDisposed = true;
                if(Obtained) {
                    locker.Unlock(Name, internalLock);
                }
            }
        }

    }
}
