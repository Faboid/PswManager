using System;
using System.Collections.Generic;

namespace PswManager.Utils {
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

    }
}
