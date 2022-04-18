using PswManagerAsync.Locks;
using PswManagerTests.Async.TestsHelpers;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Async.Locks {
    public class NamesLockerHandlerTests {

        [Fact]
        public async Task LocksSuccessfullyAndWaits() {

            //arrange
            using NamesLockerHandler locker = new();
            OrderChecker orderChecker = new();
            string lockName = "Hello!";

            //act
            var first = locker.LockHereAsync(lockName, async () => {
                await Task.Delay(100);
                orderChecker.Done(1);
            }, 1000);

            //tests timeout mechanic
            LockResult secondResult = locker.LockHere(lockName, () => {
                OrderChecker.Never();
            }, 20);

            Task<LockResult> third = locker.LockHereAsync(lockName, () => {
                orderChecker.Done(2);
            }, 1000);

            LockResult thirdResult = await third;
            LockResult firstResult = await first;

            //tests x.LockHere with no returns
            LockResult fourthResult = locker.LockHere(lockName, () => orderChecker.Done(3), 50);
            orderChecker.Done(4);

            //assert
            Assert.True(firstResult.Success);
            Assert.True(secondResult.Failed);
            Assert.True(thirdResult.Success);
            Assert.True(fourthResult.Success);

        }

    }
}
