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
            OrderChecker orderChecker = new();

            //act
            async Task Writer() {
                await channel.WriteAsync(expected[0]);
                await channel.WriteAsync(expected[1]);
                orderChecker.Done(1);
                await channel.WriteAsync(expected[2]);
                await channel.WriteAsync(expected[3]);
                orderChecker.Done(3);
                //extra values
                await channel.WriteAsync(10);
                await channel.WriteAsync(99);
                orderChecker.Done(5);
            }

            async Task Reader() {
                await orderChecker.WaitForAsync(1, 200);
                actual[0] = await TryWaitRead(channel);
                orderChecker.Done(2);
                actual[1] = await channel.ReadAsync();
                await orderChecker.WaitForAsync(3, 100);
                actual[2] = await channel.ReadAsync();
                orderChecker.Done(4);
                actual[3] = await channel.ReadAsync();
            }

            //act
            var writerTask = Writer();
            var readerTask = Reader();

            //assert
            await Task.WhenAll(writerTask, readerTask).ThrowIfTakesOver(1000);
            Assert.Equal(expected, actual);

        }


        private static async Task<T> TryWaitRead<T>(Channel<T> channel) {
            while(true) {
                var (success, value) = await channel.TryReadAsync(500).ConfigureAwait(false);

                if(success) {
                    return value;
                }
            }
        }
    }
}
