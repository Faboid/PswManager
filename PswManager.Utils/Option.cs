using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("PswManager.Utils.Tests")]
namespace PswManager.Utils;

//The code for these classes has been derived from https://dev.to/ntreu14/let-s-implement-an-option-type-in-c-1ibn

internal interface IOption<TValue, TError> {

    T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none);
    Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func);
    TValue Or(TValue def);

}

public static class Option {

    public static Option<TValue, TError> Some<TValue, TError>(TValue value) => new(value);
    public static Option<TValue, TError> Error<TValue, TError>(TError error) => new(error);
    public static Option<TValue, TError> None<TValue, TError>() => new();

}

public struct Option<TValue, TError> {

    private readonly IOption<TValue, TError> option = new None<TValue, TError>();

    public Option(TValue value) {
        option = (value != null)? 
            new Some<TValue, TError>(value) :
            new None<TValue, TError>();
    }

    public Option(TError error) {
        option = (error != null)? 
            new Error<TValue, TError>(error) :
            new None<TValue, TError>();
    }

    public Option() {
        option = new None<TValue, TError>();
    }

    private Option(IOption<TValue, TError> option) {
        this.option = option;
    }

    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => (option != null) ? option.Match(some, error, none) : none.Invoke();
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => option?.Bind(func) ?? new None<T, TError>();
    public TValue Or(TValue def) => option.Or(def);

    //static constructors
    public static Option<TValue, TError> Some(TValue value) => new(value);
    public static Option<TValue, TError> Error(TError error) => new(error);
    public static Option<TValue, TError> None() => new();

    //implicit operators
    public static implicit operator Option<TValue, TError>(Some<TValue, TError> some) => new(some);
    public static implicit operator Option<TValue, TError>(Error<TValue, TError> error) => new(error);
    public static implicit operator Option<TValue, TError>(None<TValue, TError> none) => new(none);
    public static implicit operator Option<TValue, TError>(TValue value) => new(value);
    public static implicit operator Option<TValue, TError>(TError error) => new(error);
    //default implicits to None<>

}

public struct Some<TValue, TError> : IOption<TValue, TError> {

    private readonly TValue value;

    public Some(TValue value) {
        this.value = value;
    }

    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => some.Invoke(value);
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => func.Invoke(value);
    public TValue Or(TValue def) => value;

    public static implicit operator Some<TValue, TError>(TValue value) => new(value);

}

public struct Error<TValue, TError> : IOption<TValue, TError> {

    private readonly TError err;

    public Error(TError error) {
        err = error;
    }

    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => error.Invoke(err);
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => new Error<T, TError>(err);
    public TValue Or(TValue def) => def;

    public static implicit operator Error<TValue, TError>(TError error) => new(error);

}

public struct None<TValue, TError> : IOption<TValue, TError> {

    public None() { }

    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => none.Invoke();
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => new None<T, TError>();
    public TValue Or(TValue def) => def;

}