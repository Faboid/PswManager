using System;
using System.Threading.Tasks;

namespace PswManager.Utils.Options;

public struct Some<TValue> : IOption<TValue> {

    private readonly TValue value;

    public Some(TValue value) {
        this.value = value;
    }

    public OptionResult Result() => OptionResult.Some;
    public T Match<T>(Func<TValue, T> some, Func<T> none) => some.Invoke(value);
    public Option<T> Bind<T>(Func<TValue, Option<T>> func) => func.Invoke(value);
    public async Task<Option<T>> BindAsync<T>(Func<TValue, Task<Option<T>>> func) => await func.Invoke(value);
    public TValue Or(TValue def) => value;
    public TValue OrDefault() => value;

    public static implicit operator Some<TValue>(TValue value) => new(value);

}

public struct Some<TValue, TError> : IOption<TValue, TError> {

    private readonly TValue value;

    public Some(TValue value) {
        this.value = value;
    }

    public OptionResult Result() => OptionResult.Some;
    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => some.Invoke(value);
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => func.Invoke(value);
    public async Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => await func.Invoke(value);
    public Task<Option<TValue, T>> BindErrorAsync<T>(Func<TError, Task<Option<TValue, T>>> func) => Task.FromResult<Option<TValue, T>>(new Some<TValue, T>(value));
    public TValue Or(TValue def) => value;
    public TValue OrDefault() => value;
    public TError OrError(TError def) => def;
    public TError OrDefaultError() => default;
    public Option<TValue, T> BindError<T>(Func<TError, Option<TValue, T>> func) => throw new NotImplementedException();

    public static implicit operator Some<TValue, TError>(TValue value) => new(value);

}
