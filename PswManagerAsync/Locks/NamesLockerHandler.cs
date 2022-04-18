namespace PswManagerAsync.Locks {
    public class NamesLockerHandler : IDisposable {

        private const string defaultTimeoutMessage = "This is being used elsewhere.";
        private readonly NamesLocker lockers = new();

        public async Task<LockResult<T>> LockHereAsync<T>(string name, Func<Task<T>> func, int millisecondsTimeout = -1, string timeoutMessage = defaultTimeoutMessage) {
            var lockResult = await lockers.LockAsync(name, millisecondsTimeout, timeoutMessage);
            if(lockResult.Failed) {
                return new(lockResult.ErrorMessage);
            }

            try {
                return new(await func.Invoke());
            } finally {
                lockers.Unlock(name);
            }
        }

        public async Task<LockResult> LockHereAsync(string name, Func<Task> action, int millisecondsTimeout = -1, string timeoutMessage = defaultTimeoutMessage) {
            var lockResult = await lockers.LockAsync(name, millisecondsTimeout, timeoutMessage);
            if(lockResult.Failed) {
                return lockResult;
            }

            try {
                await action.Invoke();
                return lockResult;
            } finally {
                lockers.Unlock(name);
            }
        }

        public async Task<LockResult<T>> LockHereAsync<T>(string name, Func<T> func, int millisecondsTimeout = -1, string timeoutMessage = defaultTimeoutMessage) {
            var lockResult = await lockers.LockAsync(name, millisecondsTimeout, timeoutMessage);
            return FuncLogic(name, func, lockResult);
        }

        public async Task<LockResult> LockHereAsync(string name, Action action, int millisecondsTimeout = -1, string timeoutMessage = defaultTimeoutMessage) {
            var lockResult = await lockers.LockAsync(name, millisecondsTimeout, timeoutMessage);
            return ActionLogic(name, action, lockResult);
        }

        public LockResult<T> LockHere<T>(string name, Func<T> func, int millisecondsTimeout = -1, string timeoutMessage = defaultTimeoutMessage) {
            var lockResult = lockers.Lock(name, millisecondsTimeout, timeoutMessage);
            return FuncLogic(name, func, lockResult);
        }

        public LockResult LockHere(string name, Action action, int millisecondsTimeout = -1, string timeoutMessage = defaultTimeoutMessage) {
            var lockResult = lockers.Lock(name, millisecondsTimeout, timeoutMessage);
            return ActionLogic(name, action, lockResult);
        }

        private LockResult<T> FuncLogic<T>(string name, Func<T> func, LockResult lockResult) {
            if(lockResult.Failed) {
                return new(lockResult.ErrorMessage);
            }

            try {
                return new(func.Invoke());
            } finally {
                lockers.Unlock(name);
            }
        }

        private LockResult ActionLogic(string name, Action action, LockResult lockResult) {
            if(lockResult.Failed) {
                return lockResult;
            }

            try {
                action.Invoke();
                return lockResult;
            } finally {
                lockers.Unlock(name);
            }
        }

        public void Dispose() {
            lockers.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
