namespace PswManager.Async.Locks; 
public class LockResult {

    internal LockResult(bool success) {
        Success = success;
        ErrorMessage = "None";
    }

    internal LockResult(string errorMessage) {
        ErrorMessage = errorMessage;
        Success = false;
    }

    public bool Success { get; init; }
    public bool Failed => !Success;
    public string ErrorMessage { get; init; }

    internal static LockResult CreateResult(bool success, string errorMessageIfFailure) => success switch {
        true => new LockResult(true),
        false => new LockResult(errorMessageIfFailure)
    };

}

public class LockResult<T> : LockResult {

    internal LockResult(string errorMessage) : base(errorMessage) { }
    internal LockResult(T value) : base(true) {
        Value = value;
    }

    public T? Value { get; init; }

}
