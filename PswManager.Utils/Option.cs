using System;
namespace PswManager.Utils;

public interface IOption<TValue, TError> {

    T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none);
    IOption<T, TError> Next<T>(Func<TValue, IOption<T, TError>> func);

}

public struct Option<TValue, TError> : IOption<TValue, TError> {

    private readonly IOption<TValue, TError> option = new None<TValue, TError>();

    public Option(TValue value) {
        option = new Some<TValue, TError>(value);
    }

    public Option(TError error) {
        option = new Error<TValue, TError>(error);
    }

    public Option() {
        option = new None<TValue, TError>();
    }

    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => option.Match(some, error, none);
    public IOption<T, TError> Next<T>(Func<TValue, IOption<T, TError>> func) => option.Next(func);

}

internal class Some<TValue, TError> : IOption<TValue, TError> {

    private readonly TValue value;

    internal Some(TValue value) {
        this.value = value;
    }

    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => some.Invoke(value);
    public IOption<T, TError> Next<T>(Func<TValue, IOption<T, TError>> func) => func.Invoke(value);

}

internal class Error<TValue, TError> : IOption<TValue, TError> {

    private readonly TError err;

    public Error(TError error) {
        err = error;
    }

    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => error.Invoke(err);
    public IOption<T, TError> Next<T>(Func<TValue, IOption<T, TError>> func) => new Error<T, TError>(err);

}

internal class None<TValue, TError> : IOption<TValue, TError> {

    public None() { }

    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => none.Invoke();
    public IOption<T, TError> Next<T>(Func<TValue, IOption<T, TError>> func) => new None<T, TError>();
}