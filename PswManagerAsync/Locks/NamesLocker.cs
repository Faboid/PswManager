using System.Collections.Concurrent;
using PswManagerHelperMethods.WrappingObjects;

namespace PswManagerAsync.Locks {
    public class NamesLocker : IDisposable {

        readonly ConcurrentDictionary<string, RefCount<Locker>> lockers = new();

        /// <summary>
        /// This locker is used to obtain a generic lock on ALL lockers.
        /// The logic surrounding it is a bit messy, but it was the best I could come up with.
        /// </summary>
        readonly Locker mainLocker = new();

        /// <summary>
        /// Used to synchronize access to <see cref="lockers"/>.
        /// </summary>
        readonly Locker synchronizationLocker = new();
        private bool isDisposed = false;

        public async Task<MainLock> GetAllLocksAsync(int millisecondsTimeout = -1) {
            var mainLock = await mainLocker.GetLockAsync(millisecondsTimeout).ConfigureAwait(false);
            if(mainLock.Obtained == false) {
                return new(mainLock, new(), this);
            }

            var listLocksTasks = lockers
                .AsParallel()
                .Select(async x => {
                    using var reference = x.Value.GetRef();
                    return new Lock(
                        x.Key,
                        await reference.Value.GetLockAsync(millisecondsTimeout).ConfigureAwait(false),
                        this
                    );
                });

            var listLocks = (await Task.WhenAll(listLocksTasks).ConfigureAwait(false)).ToList();
            return ManageLocksToCreateMainLock(mainLock, listLocks);
        }

        public MainLock GetAllLocks(int millisecondsTimeout = -1) {
            var mainLock = mainLocker.GetLock(millisecondsTimeout);
            if(mainLock.Obtained == false) {
                return new(mainLock, new(), this);
            }

            var listLocks = lockers
                .AsParallel()
                .Select(x => {
                    using var reference = x.Value.GetRef();
                    return new Lock(
                        x.Key,
                        reference.Value.GetLock(millisecondsTimeout),
                        this
                    );
                })
                .ToList();

            return ManageLocksToCreateMainLock(mainLock, listLocks);
        }

        private MainLock ManageLocksToCreateMainLock(Locker.Lock mainLock, List<Lock> listLocks) {
            //if any of the locks has failed to be acquired, the method has failed to lock
            //therefore, every lock should be freed and the list cleared
            if(listLocks.Any(x => x.Obtained == false)) {
                listLocks.ForEach(x => x.Dispose());
                listLocks.Clear();
                mainLock.Dispose();
                return new(this);
            }

            return new(mainLock, listLocks, this);
        }

        public async Task<Lock> GetLockAsync(string name, int millisecondsTimeout = -1) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("The given name is null, empty, or white space.", nameof(name));
            }

            var allLock = await mainLocker.GetLockAsync(millisecondsTimeout).ConfigureAwait(false);
            if(!allLock.Obtained) {
                return new(name, allLock, this);
            }

            using var reference = GetRefLocker(name, allLock);
            var heldLock = await reference.Value.GetLockAsync(millisecondsTimeout).ConfigureAwait(false);
            return new(name, heldLock, this);
        }

        public Lock GetLock(string name, int millisecondsTimeout = -1) {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("The given name is null, empty, or white space.", nameof(name));
            }

            var allLock = mainLocker.GetLock(millisecondsTimeout);
            if(!allLock.Obtained) {
                return new(name, allLock, this);
            }

            using var refLocker = GetRefLocker(name, allLock);
            var heldLock = refLocker.Value.GetLock(millisecondsTimeout);
            return new(name, heldLock, this);
        }

        private void Unlock(string name, Locker.Lock internalLock) {
            using var getLock = synchronizationLocker.GetLock();
            lock(lockers) {

                if(!lockers.TryGetValue(name, out var refLocker)) {
                    //if there's none, return
                    return;
                }

                lock(refLocker) {

                    //if it's not being used, dispose it
                    if(!refLocker.IsInUse) {
                        lockers.TryRemove(name, out _);
                        using var reference = refLocker.GetRef();
                        reference.Value.Dispose();
                        return;
                    }

                    //if it's being used, simply release it
                    internalLock.Dispose();

                }

            }
        }

        private RefCount<Locker>.Ref GetRefLocker(string name, Locker.Lock mainLock) {
            if(isDisposed) {
                throw new ObjectDisposedException($"The {nameof(NamesLocker)} object has been already disposed of.");
            }

            using var syncLock = synchronizationLocker.GetLock();
            Locker defaultLocker = new(1);
            RefCount<Locker> refSlim = new(defaultLocker);
            var refLocker = lockers.GetOrAdd(name, refSlim);

            //if the dictionary returns an existing value,
            //the newly-created locker is redundant and must be disposed of
            if(!ReferenceEquals(refSlim, refLocker)) {
                defaultLocker.Dispose();
            }

            //by unlocking the lock after getting the reference, we protect from race conditions for the disposal of locks
            var output = refLocker.GetRef();
            mainLock.Dispose();

            return output;
        }

        public void Dispose() {
            using(var syncLock = synchronizationLocker.GetLock()) {
                isDisposed = true;
                Parallel.ForEach(lockers, refVal => {
                    using var reference = refVal.Value.GetRef();
                    reference.Value.Dispose();
                });
                lockers.Clear();
            }
            synchronizationLocker.Dispose();
            GC.SuppressFinalize(this);
        }

        public class Lock : IDisposable {

            private readonly NamesLocker locker;
            private readonly Locker.Lock internalLock;
            private bool isDisposed = false;

            public bool Obtained { get; init; }
            public string Name { get; init; }

            internal Lock(string name, Locker.Lock internalLock, NamesLocker locker) {
                Name = name;
                this.internalLock = internalLock;
                this.locker = locker;
                Obtained = internalLock.Obtained;
            }

            public void Dispose() {
                if(isDisposed) {
                    throw new ObjectDisposedException(nameof(Lock));
                }

                isDisposed = true;
                if(Obtained) {
                    locker.Unlock(Name, internalLock);
                }
                GC.SuppressFinalize(this);
            }
        }

        public class MainLock : IDisposable {

            private readonly NamesLocker locker;
            private readonly Locker.Lock mainLock;
            private readonly List<Lock> internalLocks;
            private bool isDisposed = false;

            public bool Obtained { get; init; }
            
            internal MainLock(NamesLocker locker) {
                Obtained = false;
                this.locker = locker;
                mainLock = default;
                internalLocks = new();
            }

            internal MainLock(Locker.Lock mainLock, List<Lock> internalLocks, NamesLocker locker) {
                this.mainLock = mainLock;
                this.internalLocks = internalLocks;
                this.locker = locker;
                Obtained = mainLock.Obtained;
            }

            public void Dispose() {
                if(isDisposed) {
                    throw new ObjectDisposedException(nameof(MainLock));
                }
                isDisposed = true;

                if(Obtained) {
                    mainLock.Dispose();
                    internalLocks.ForEach(x => x.Dispose());
                }
                GC.SuppressFinalize(this);
            }

        }

    }
}
