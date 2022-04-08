using PswManagerAsync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Async {
    public class ChannelTests {

        [Fact]
        public async void ChannelDoesNotLock() {

            //arrange
            Channel<string> channel = new Channel<string>(3);
            string[] expected = { "First", "Second", "Third" };
            string[] actual = new string[3];

            //act
            var task = new Task(async () => {
                await channel.WriteAsync(expected[0]);
                (_, actual[0]) = await channel.TryReadAsync(1000);
                await channel.WriteAsync(expected[1]);
                await channel.WriteAsync(expected[2]);
                actual[1] = await channel.ReadAsync();
                actual[2] = await channel.ReadAsync();
            });
            task.Start();

            //logic to wait for the task's end taken from https://stackoverflow.com/questions/4238345/asynchronously-wait-for-taskt-to-complete-with-timeout/11191070#11191070
            if(await Task.WhenAny(task, Task.Delay(1000)) != task) {

                //if the delay ended before(and thus the task is in a deadlock)
                throw new TimeoutException("The task is probably in a deadlock. The test has failed.");
            }

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public async void CorrectOrder() {

            //todo

        }


    }
}
