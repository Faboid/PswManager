﻿namespace PswManager.Extensions;
public static class IAsyncEnumerableExtensions {

    public static async IAsyncEnumerable<T> Take<T>(this IAsyncEnumerable<T> enumerable, int count) {
        int curr = 0;
        await foreach(var item in enumerable.ConfigureAwait(false)) {
            if(curr >= count) {
                break;
            }
            yield return item;
            curr++;
        }
    }

    public static async Task<List<T>> ToList<T>(this IAsyncEnumerable<T> enumerable) {
        List<T> list = new();
        await foreach(var item in enumerable.ConfigureAwait(false)) {
            list.Add(item);
        }
        return list;
    }

    public static async IAsyncEnumerable<U> Select<T, U>(this IAsyncEnumerable<T> enumerable, Func<T, U> selector) {
        await foreach(var item in enumerable.ConfigureAwait(false)) {
            yield return selector(item);
        }
    }

    public static async IAsyncEnumerable<U> Select<T, U>(this IAsyncEnumerable<T> enumerable, Func<T, Task<U>> selector) {
        await foreach(var item in enumerable.ConfigureAwait(false)) {
            yield return await selector(item).ConfigureAwait(false);
        }
    }

    public static async Task<string> JoinStrings(this IAsyncEnumerable<string> enumerable, string separator) {
        List<string> values = new();
        await foreach(var s in enumerable.ConfigureAwait(false)) {
            values.Add(s);
        }

        return string.Join(separator, values);
    }

}
