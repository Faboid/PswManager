using System;
using System.Threading.Tasks;

namespace PswManagerTests.Async.TestsHelpers {
    public static class TimeoutProtection {

        public static async Task<T> ThrowIfTakesOver<T>(this Task<T> task, int milliseconds) {
            await task.TimeoutIfOver(milliseconds);
            return await task;
        }

        public static async Task ThrowIfTakesOver(this Task task, int milliseconds) {
            await task.TimeoutIfOver(milliseconds);
            await task;
        }

        private static async Task TimeoutIfOver(this Task task, int milliseconds) {
            //logic to wait for the task's end taken from https://stackoverflow.com/questions/4238345/asynchronously-wait-for-taskt-to-complete-with-timeout/11191070#11191070
            Task waitTask = Task.Delay(milliseconds);
            if(await Task.WhenAny(task, waitTask) != task) {
                throw new TimeoutException("The given task has timed out.");
            }
        }

    }
}
