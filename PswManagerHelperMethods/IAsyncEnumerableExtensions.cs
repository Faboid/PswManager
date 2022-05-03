using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerHelperMethods {
    public static class IAsyncEnumerableExtensions {

        public static async Task<List<T>> Take<T>(this IAsyncEnumerable<T> enumerable, int count) {
            List<T> list = new();

            int curr = 0;
            await foreach(var item in enumerable.ConfigureAwait(false)) {
                if(curr >= count) {
                    break;
                }
                list.Add(item);
                curr++;
            }

            return list;
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

    }
}
