using PswManager.Utils.Options;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly:InternalsVisibleTo("PswManager.Utils.Tests")]
namespace PswManager.Utils;

public static class Option {

    public static Option<TValue> Some<TValue>(TValue value) => new(value);
    public static Option<TValue> None<TValue>() => new();

    public static Option<TValue, TError> Some<TValue, TError>(TValue value) => new(value);
    public static Option<TValue, TError> Error<TValue, TError>(TError error) => new(error);
    public static Option<TValue, TError> None<TValue, TError>() => new();

}

public struct Option<TValue> : IOption<TValue> {

    private IOption<TValue> GetOption => _option ?? new None<TValue>();
    private readonly IOption<TValue> _option;

    public Option(TValue value) {
        _option = (value != null) ?
            new Some<TValue>(value) :
            new None<TValue>();
    }

    public Option() {
        _option = new None<TValue>();
    }

    private Option(IOption<TValue> option) {
        _option = option;
    }

    public OptionResult Result() => GetOption.Result();
    public T Match<T>(Func<TValue, T> some, Func<T> none) => GetOption.Match(some, none);
    public Option<T> Bind<T>(Func<TValue, Option<T>> func) => GetOption.Bind(func);
    public async Task<Option<T>> BindAsync<T>(Func<TValue, Task<Option<T>>> func) => await GetOption.BindAsync(func).ConfigureAwait(false);
    public TValue Or(TValue def) => GetOption.Or(def);
    public TValue OrDefault() => GetOption.OrDefault();


    //static constructors
    public static Option<TValue> Some(TValue value) => new(value);
    public static Option<TValue> None() => new();

    //implicit operators
    public static implicit operator Option<TValue>(Some<TValue> some) => new(some);
    public static implicit operator Option<TValue>(None<TValue> none) => new(none);
    public static implicit operator Option<TValue>(TValue value) => new(value);
    //default implicits to None<>

}

public struct Option<TValue, TError> : IOption<TValue, TError> {

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

    public OptionResult Result() => GetOption.Result();
    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => GetOption.Match(some, error, none);
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => GetOption.Bind(func);
    public async Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => await GetOption.BindAsync(func).ConfigureAwait(false);
    public Option<TValue, T> BindError<T>(Func<TError, Option<TValue, T>> func) => GetOption.BindError(func);
    public Task<Option<TValue, T>> BindErrorAsync<T>(Func<TError, Task<Option<TValue, T>>> func) => GetOption.BindErrorAsync(func);
    public TValue Or(TValue def) => GetOption.Or(def);
    public TValue OrDefault() => GetOption.OrDefault();
    public TError OrError(TError def) => GetOption.OrError(def);
    public TError OrDefaultError() => GetOption.OrDefaultError();


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