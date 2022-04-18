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
            using OrderChecker orderChecker = new();
            string lockName = "Hello!";

            //act
            var first = locker.LockHereAsync(lockName, async () => {
                await orderChecker.WaitFor(1, 200);
                orderChecker.Done(2);
                return Task.FromResult(2);
            }, 1000);

            //tests timeout mechanic
            LockResult<int> secondResult = locker.LockHere(lockName, () => {
                OrderChecker.Never();
                return 5;
            }, 10);

            Task<LockResult<int>> third = locker.LockHereAsync(lockName, () => {
                orderChecker.Done(3);
                return 4;
            }, 1000);

            orderChecker.Done(1);
            LockResult<int> thirdResult = await third;
            LockResult firstResult = await first;

            //tests x.LockHere with no returns
            LockResult fourthResult = locker.LockHere(lockName, () => orderChecker.Done(4), 50);
            orderChecker.Done(5);

            //assert
            Assert.True(firstResult.Success);
            Assert.True(secondResult.Failed);
            Assert.True(thirdResult.Success);
            Assert.True(fourthResult.Success);

        }

        [Fact]
        public async Task DifferentiatesBetweenNames() {

            //arrange
            using NamesLockerHandler locker = new();
            using OrderChecker orderChecker = new();
            string lock1 = "One";
            string lock2 = "Two";

            //act
            var firstLock = locker
                .LockHereAsync(lock1, async () => {
                        await orderChecker.WaitFor(1, 500);
                        orderChecker.Done(2);
                        await orderChecker.WaitFor(3, 500);
                    }, 100)
                .ThrowIfTakesOver(1000);

            var firstShouldBeLocked = locker
                .LockHereAsync(lock1, () => OrderChecker.Never(), 10)
                .ThrowIfTakesOver(1000);

            var secondResult = locker
                .LockHere(lock2, () => orderChecker.Done(1), 50);

            await orderChecker.WaitFor(2, 500);
            orderChecker.Done(3);
            var firstResult = await firstLock;
            var firstShouldBeLockedResult = await firstShouldBeLocked;
            orderChecker.Done(4);

            //assert
            Assert.True(firstResult.Success);
            Assert.True(firstShouldBeLockedResult.Failed);
            Assert.True(secondResult.Success);

        }

    }
}
