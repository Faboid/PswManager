using System;

namespace PswManager.Utils;

public abstract class AbstractResult<TEnum> where TEnum : Enum {

    /// <summary>
    /// Creates a result and assigns the default success/failure code based on <paramref name="success"/>.
    /// </summary>
    /// <param name="success"></param>
    public AbstractResult(bool success) {
        ResultCode = success switch {
            true => SuccessCode,
            false => FailureCode,
        };

        Success = success;
    }

    public AbstractResult(TEnum errorCode) {
        ResultCode = errorCode;
        Success = IsSuccess(errorCode);
    }

    public bool Success { get; init; }
    public bool Failure => !Success;
    public TEnum ResultCode { get; init; }

    /// <summary>
    /// The code that will be assigned when the result is successful.
    /// </summary>
    /// <returns></returns>
    protected abstract TEnum SuccessCode { get; }

    /// <summary>
    /// Generic failure code that is to be assigned when no specific failure type is assigned.
    /// </summary>
    /// <returns></returns>
    protected abstract TEnum FailureCode { get; }

    private bool IsSuccess(TEnum code) {
        return code.IsEqual(SuccessCode);
    }

}

public abstract class AbstractResult<TEnum, TValue> : AbstractResult<TEnum> where TEnum : Enum {

    /// <summary>
    /// Creates a result with a specific result code. Note that it's illegal to create a successful result without a value.
    /// Using <see cref="SuccessCode"/> as the value will throw a <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="result"></param>
    /// <exception cref="ArgumentException"></exception>
    public AbstractResult(TEnum result) : base(result) {
        if(result.IsEqual(SuccessCode)) {
            throw new ArgumentException("The result given through the AbstractResult(TEnum) constructor cannot be successful. " +
                "If you want to specify a 'type of success', use the AbstractResult(TValue, TEnum) constructor instead.");
        }
    }

    /// <summary>
    /// Creates a result by giving it a value and a specific result code.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="result"></param>
    public AbstractResult(TValue value, TEnum result) : base(result) {
        Value = value;
    }

    /// <summary>
    /// Create a result by giving it a value. The result will be considered a success, even if the value is null.
    /// </summary>
    /// <param name="value"></param>
    public AbstractResult(TValue value) : base(true) {
        Value = value;
    }

    /// <summary>
    /// The value returned by the result.
    /// If the result is that of failure, this value will be <see langword="default"/>.
    /// </summary>
    public TValue Value { get; init; }

    protected override abstract TEnum FailureCode { get; }

    protected override abstract TEnum SuccessCode { get; }

}
