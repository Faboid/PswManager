using System;
using System.Threading;
using System.Threading.Tasks;

namespace PswManager.Utils.WrappingObjects;


public class AsyncResult : Task<Result> {
    public AsyncResult(Func<Result> function) 
        : base(function) { }

    public AsyncResult(Func<object, Result> function, object state) 
        : base(function, state) { }

    public AsyncResult(Func<Result> function, CancellationToken cancellationToken) 
        : base(function, cancellationToken) { }

    public AsyncResult(Func<Result> function, TaskCreationOptions creationOptions) 
        : base(function, creationOptions) { }

    public AsyncResult(Func<object, Result> function, object state, CancellationToken cancellationToken) 
        : base(function, state, cancellationToken) { }

    public AsyncResult(Func<object, Result> function, object state, TaskCreationOptions creationOptions) 
        : base(function, state, creationOptions) { }

    public AsyncResult(Func<Result> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions) 
        : base(function, cancellationToken, creationOptions) { }

    public AsyncResult(Func<object, Result> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) 
        : base(function, state, cancellationToken, creationOptions) { }
}

public class AsyncResult<T> : Task<Result<T>> {
    public AsyncResult(Func<Result<T>> function) 
        : base(function) { }

    public AsyncResult(Func<object, Result<T>> function, object state) 
        : base(function, state) { }

    public AsyncResult(Func<Result<T>> function, CancellationToken cancellationToken) 
        : base(function, cancellationToken) { }

    public AsyncResult(Func<Result<T>> function, TaskCreationOptions creationOptions) 
        : base(function, creationOptions) { }

    public AsyncResult(Func<object, Result<T>> function, object state, CancellationToken cancellationToken) 
        : base(function, state, cancellationToken) { }

    public AsyncResult(Func<object, Result<T>> function, object state, TaskCreationOptions creationOptions) 
        : base(function, state, creationOptions) { }

    public AsyncResult(Func<Result<T>> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions) 
        : base(function, cancellationToken, creationOptions) { }

    public AsyncResult(Func<object, Result<T>> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) 
        : base(function, state, cancellationToken, creationOptions) { }
}

