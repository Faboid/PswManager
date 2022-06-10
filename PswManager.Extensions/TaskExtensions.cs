namespace PswManager.Extensions; 

public static class TaskExtensions {

    public static Task<T> AsTask<T>(this T value) => Task.FromResult(value);
    public static ValueTask<T> AsValueTask<T>(this T value) => ValueTask.FromResult(value);

}
