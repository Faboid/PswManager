using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly:InternalsVisibleTo("PswManager.Utils.Tests")]
namespace PswManager.Utils;

//The code for these classes has been derived from https://dev.to/ntreu14/let-s-implement-an-option-type-in-c-1ibn

internal interface IOption<TValue, TError> {

    T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none);
    Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func);
    Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func);
    TValue Or(TValue def);

}

public static class Option {

    public static Option<TValue, TError> Some<TValue, TError>(TValue value) => new(value);
    public static Option<TValue, TError> Error<TValue, TError>(TError error) => new(error);
    public static Option<TValue, TError> None<TValue, TError>() => new();

}

public struct Option<TValue, TError> {

    private IOption<TValue, TError> GetOption => _option ?? new None<TValue, TError>();
    private readonly IOption<TValue, TError> _option;

    public Option(TValue value) {
        _option = (value != null)? 
            new Some<TValue, TError>(value) :
            new None<TValue, TError>();
    }

    public Option(TError error) {
        _option = (error != null)? 
            new Error<TValue, TError>(error) :
            new None<TValue, TError>();
    }

    public Option() {
        _option = new None<TValue, TError>();
    }

    private Option(IOption<TValue, TError> option) {
        _option = option;
    }

    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => GetOption.Match(some, error, none);
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => GetOption.Bind(func);
    public async Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => await GetOption.BindAsync(func).ConfigureAwait(false);
    public TValue Or(TValue def) => GetOption.Or(def);

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
    public async Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => await func.Invoke(value);
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
    public Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => Task.FromResult<Option<T, TError>>(new Error<T, TError>(err));
    public TValue Or(TValue def) => def;


    public static implicit operator Error<TValue, TError>(TError error) => new(error);

}

public struct None<TValue, TError> : IOption<TValue, TError> {

    public None() { }

    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => none.Invoke();
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => new None<T, TError>();
    public Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => Task.FromResult<Option<T, TError>>(new None<T, TError>());
    public TValue Or(TValue def) => def;

}