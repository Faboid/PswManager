using System;
using System.Threading.Tasks;

namespace PswManager.Utils.Options;

public struct None<TValue> : IOption<TValue> {

    public None() { }

    public OptionResult Result() => OptionResult.None;
    public T Match<T>(Func<TValue, T> some, Func<T> none) => none.Invoke();
    public Option<T> Bind<T>(Func<TValue, Option<T>> func) => new None<T>();
    public Task<Option<T>> BindAsync<T>(Func<TValue, Task<Option<T>>> func) => Task.FromResult<Option<T>>(new None<T>());
    public TValue Or(TValue def) => def;
    public TValue OrDefault() => default;
}

public struct None<TValue, TError> : IOption<TValue, TError> {

    public None() { }

    public OptionResult Result() => OptionResult.None;
    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => none.Invoke();
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => new None<T, TError>();
    public Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => Task.FromResult<Option<T, TError>>(new None<T, TError>());
    public Option<TValue, T> BindError<T>(Func<TError, Option<TValue, T>> func) => new None<TValue, T>();
    public Task<Option<TValue, T>> BindErrorAsync<T>(Func<TError, Task<Option<TValue, T>>> func) => Task.FromResult<Option<TValue, T>>(new None<TValue, T>());
    public TValue Or(TValue def) => def;
    public TValue OrDefault() => default;
    public TError OrError(TError def) => def;
    public TError OrDefaultError() => default;
}
