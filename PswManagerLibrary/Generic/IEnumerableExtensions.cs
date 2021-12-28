using System;
using System.Collections.Generic;

namespace PswManagerLibrary.Generic {
    public static class IEnumerableExtensions {

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T> action) {
            foreach(T item in enumeration) {
                action.Invoke(item);
            }
            return enumeration;
        }

    }
}
