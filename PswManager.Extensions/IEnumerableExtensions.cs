namespace PswManager.Extensions;
public static class IEnumerableExtensions {

    /// <summary>
    /// Enumerates the given enumerable, executes an action for each item, and returns the enumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumeration"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T> action) {
        foreach(T item in enumeration) {
            action.Invoke(item);
        }
        return enumeration;
    }

    /// <summary>
    /// <inheritdoc cref="string.Join{T}(char, IEnumerable{T})"/>
    /// </summary>
    /// <param name="enumeration"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string JoinStrings(this IEnumerable<string> enumeration, char separator) {
        return string.Join(separator, enumeration);
    }

    /// <summary>
    /// <inheritdoc cref="string.Join(string?, IEnumerable{string?})"/>
    /// </summary>
    /// <param name="enumeration"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string JoinStrings(this IEnumerable<string> enumeration, string separator) {
        return string.Join(separator, enumeration);
    }

    /// <summary>
    /// Projects the <see cref="IEnumerable{T}"/> into an <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<Task<T>> enumerable) {
        foreach(var value in enumerable) {
            yield return await value.ConfigureAwait(false);
        }
    }

}
