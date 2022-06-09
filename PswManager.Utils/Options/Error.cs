using System;
using System.Threading.Tasks;

namespace PswManager.Utils.Options; 

public struct Error<TValue, TError> : IOption<TValue, TError> {

    private readonly TError err;

    public Error(TError error) {
        err = error;
    }

    public OptionResult Result() => OptionResult.Error;
    public T Match<T>(Func<TValue, T> some, Func<TError, T> error, Func<T> none) => error.Invoke(err);
    public Option<T, TError> Bind<T>(Func<TValue, Option<T, TError>> func) => new Error<T, TError>(err);
    public Task<Option<T, TError>> BindAsync<T>(Func<TValue, Task<Option<T, TError>>> func) => Task.FromResult<Option<T, TError>>(new Error<T, TError>(err));
    public TValue Or(TValue def) => def;


    public static implicit operator Error<TValue, TError>(TError error) => new(error);

}
