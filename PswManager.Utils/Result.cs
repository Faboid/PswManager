namespace PswManager.Utils;

public class Result {

    public Result(bool success) {
        Success = success;
    }

    public Result(string errorMessage) {
        Success = false;
        ErrorMessage = errorMessage;
    }

    public bool Success { get; init; }
    public bool Failed => !Success;
    public string ErrorMessage { get; init; } = string.Empty;
    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

}

public class Result<TValue> : Result {

    public Result(bool success) : base(success) { }

    public Result(string errorMessage) : base(errorMessage) { }
    public Result(TValue value) : base(true) {
        Value = value;
    }

    public TValue Value { get; init; }

}
