using System;
using System.Collections.Generic;

namespace PswManager.Extensions {
    public static class IEnumerableExtensions {

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T> action) {
            foreach(T item in enumeration) {
                action.Invoke(item);
            }
            return enumeration;
        }

        public static string JoinStrings(this IEnumerable<string> enumeration, char separator) {
            return string.Join(separator, enumeration);
        }

        public static string JoinStrings(this IEnumerable<string> enumeration, string separator) {
            return string.Join(separator, enumeration);
        }

        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<Task<T>> enumerable) {
            foreach(var value in enumerable) {
                yield return await value.ConfigureAwait(false);
            }
        }

    }
}
