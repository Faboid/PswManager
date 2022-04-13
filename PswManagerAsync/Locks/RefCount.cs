namespace PswManagerAsync.Locks {
    internal class RefCount<TValue> {

        public RefCount(TValue value) {
            this.value = value;
            references = 0;
        }

        public bool IsInUse => references != 0;

        private readonly TValue value;
        private int references;

        public async Task UseValueAsync(Func<Task> predicate) {
            try {
                references++;
                await predicate.Invoke();
            }
            finally {
                references--;
            }
        }

        public async Task<T> UseValueAsync<T>(Func<TValue, Task<T>> predicate) {
            try {
                references++;
                return await predicate.Invoke(value);
            }
            finally {
                references--;
            }
        }

        public void UseValue(Action<TValue> action) {
            try {
                references++;
                action.Invoke(value);
            }
            finally {
                references--;
            }
        }

        public T UseValue<T>(Func<TValue, T> predicate) {
            try {
                references++;
                return predicate.Invoke(value);
            }
            finally {
                references--;
            }
        }

    }
}
