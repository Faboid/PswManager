namespace PswManager.Extensions;

public static class TaskExtensions {

    /// <summary>
    /// <inheritdoc cref="Task.FromResult{TResult}(TResult)"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Task<T> AsTask<T>(this T value) => Task.FromResult(value);

    /// <summary>
    /// <inheritdoc cref="ValueTask.FromResult{TResult}(TResult)"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ValueTask<T> AsValueTask<T>(this T value) => ValueTask.FromResult(value);

}
