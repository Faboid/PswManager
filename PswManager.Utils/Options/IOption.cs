using System;
using System.Threading.Tasks;

namespace PswManager.Utils.Options;

internal interface IOption<TValue, TError> {

    OptionResult Result();
    T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none);
    Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func);
    Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func);
    TValue Or(TValue def);

}

internal interface IOption<TValue> {

    OptionResult Result();
    T Match<T>(Func<TValue, T> some, Func<T> none);
    Option<T> Bind<T>(Func<TValue, Option<T>> func);
    Task<Option<T>> BindAsync<T>(Func<TValue, Task<Option<T>>> func);
    TValue Or(TValue def);

}

