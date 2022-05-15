using System.Runtime.CompilerServices;

namespace PswManagerAsync {
    public class AsyncLazy<T> : Lazy<Task<T>> {

        //this class is derived from https://devblogs.microsoft.com/pfxteam/asynclazyt/
        
        private AsyncLazy() { }
        public AsyncLazy(Func<Task<T>> func) : base (Task.Factory.StartNew(func).Unwrap()) { }
        public AsyncLazy(Func<T> valueFactory) : base(Task.Factory.StartNew(valueFactory)) { }
        public AsyncLazy(Task<T> value) : base(Task.Factory.StartNew(() => value).Unwrap()) { }
        public AsyncLazy(T value) : base(Task.FromResult(value)) {}

        public new bool IsValueCreated => Value.IsCompleted;

        public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();

        public static implicit operator AsyncLazy<T>(T value) => new(value);
        public static implicit operator AsyncLazy<T>(Task<T> value) => new(value);
        public static implicit operator AsyncLazy<T>(Func<T> valueFactory) => new(valueFactory);
        public static implicit operator AsyncLazy<T>(Func<Task<T>> func) => new(func);

        public static implicit operator Task<T>(AsyncLazy<T> lazy) => lazy.Value;
        public static implicit operator T(AsyncLazy<T> lazy) => lazy.Value.Result;

    }
}
