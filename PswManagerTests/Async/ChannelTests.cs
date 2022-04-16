using PswManagerAsync;
using PswManagerTests.Async.TestsHelpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Async {
    public class ChannelTests {

        [Fact]
        public async Task ChannelDoesNotLock() {

            //arrange
            Channel<string> channel = new Channel<string>(3);
            string[] expected = { "First", "Second", "Third" };
            string[] actual = new string[3];

            //act
            async Task TestLogic() {
                await channel.WriteAsync(expected[0]);
                (_, actual[0]) = await channel.TryReadAsync(1000);
                await channel.WriteAsync(expected[1]);
                await channel.WriteAsync(expected[2]);
                actual[1] = await channel.ReadAsync();
                actual[2] = await channel.ReadAsync();
            }

            await TestLogic().ThrowIfTakesOver(1000);

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public async void CorrectOrder() {

            //arrange
            Channel<int> channel = new(2);
            int[] expected = { 1, 2, 3, 4 };
            int[] actual = new int[4];

            //act
            Task<double> writeTask = Task.Run(async () => {
                Stopwatch sw = Stopwatch.StartNew();
                await channel.WriteAsync(expected[0]);
                await channel.WriteAsync(expected[1]);
                await channel.WriteAsync(expected[2]);
                await channel.WriteAsync(expected[3]);
                //extra values
                await channel.WriteAsync(10);
                await channel.WriteAsync(99);
                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            });
            Task<double> readTask = Task.Run(async () => {
                Stopwatch sw = Stopwatch.StartNew();
                actual[0] = await TryWaitRead(channel);
                actual[1] = await channel.ReadAsync();
                actual[2] = await channel.ReadAsync();
                await Task.Delay(200);
                actual[3] = await channel.ReadAsync();
                sw.Stop();
                return sw.Elapsed.TotalMilliseconds;
            });

            //act
            var timeoutTask = Task.Delay(1000);

            //assert
            if(await Task.WhenAny(writeTask, readTask, timeoutTask) == timeoutTask) {

                throw new TimeoutException("The task is probably in a deadlock. The test has failed.");
            }

            await Task.WhenAll(writeTask, readTask);
            //based on pre-established locks, "readTask" should finish before "writeTask",
            //as WriteAsync() should be locked until the stored values are read
            Assert.True(writeTask.Result > readTask.Result);
            Assert.Equal(expected, actual);

        }


        private static async Task<T> TryWaitRead<T>(Channel<T> channel) {
            while(true) {
                var (success, value) = await channel.TryReadAsync(500);

                if(success) {
                    return value;
                }
            }
        }
    }
}
