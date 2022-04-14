using PswManagerAsync.Locks;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Async.Locks {
    public class NamesLockerHandlerTests {

        [Fact]
        public async Task LocksSuccessfullyAndWaits() {

            //arrange
            using NamesLockerHandler locker = new();
            Stopwatch sw = Stopwatch.StartNew();
            string lockName = "Hello!";

            //act
            var first = locker.LockHereAsync(lockName, async () => {
                await Task.Delay(100);
                return sw.Elapsed.TotalMilliseconds;
            }, 1000);

            //tests timeout mechanic
            LockResult<int> secondResult = locker.LockHere(lockName, () => 5, 20);

            Task<LockResult<double>> third = locker.LockHereAsync(lockName, () => {
                return Task.FromResult(sw.Elapsed.TotalMilliseconds);
            }, 1000);

            LockResult<double> thirdResult = await third;
            LockResult<double> firstResult = await first;

            //tests x.LockHere with no returns
            LockResult fourthResult = locker.LockHere(lockName, () => sw.Stop(), 50);
            var one = sw.Elapsed.TotalMilliseconds;
            await Task.Delay(10);
            var two = sw.Elapsed.TotalMilliseconds;

            //assert
            Assert.True(firstResult.Success);
            Assert.True(secondResult.Failed);
            Assert.True(thirdResult.Success);
            Assert.True(fourthResult.Success);
            Assert.True(firstResult.Value < thirdResult.Value);
            Assert.True(one == two);

        }

    }
}
