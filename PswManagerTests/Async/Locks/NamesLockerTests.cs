using PswManagerAsync.Locks;
using PswManagerTests.Async.TestsHelpers;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Async.Locks {
    public class NamesLockerTests {

        //todo - this test is a mess. Fix it
        [Fact]
        public async Task DifferentiatesDifferentNames() {

            //arrange
            using NamesLocker locker = new();
            string lock1 = "Hello!";
            string lock2 = "Sup!";
            using OrderChecker orderChecker = new();

            //act
            async Task<(NamesLocker.Lock, NamesLocker.Lock)> PartOneLockLogic() {
                orderChecker.Done(1);
                using var heldLock1 = locker.GetLock(lock1);
                var heldLock2 = await locker.GetLockAsync(lock2);
                var failedLockResult = await locker.GetLockAsync(lock1, 10);
                return (heldLock2, failedLockResult);
            }

            async Task PartTwoLockLogic() {
                orderChecker.Done(2);
                await locker.GetLockAsync(lock2);
                orderChecker.Done(4);
            }

            async Task UnlockLogic(NamesLocker.Lock heldLock2) {
                await orderChecker.WaitFor(2, 100);
                heldLock2.Dispose();
                orderChecker.Done(3);
            }

            var lockTask = PartOneLockLogic().ThrowIfTakesOver(1000);
            var lockTaskResult = await lockTask;
            var unlockTask = UnlockLogic(lockTaskResult.Item1).ThrowIfTakesOver(1000);
            var secondPart = PartTwoLockLogic().ThrowIfTakesOver(1000);

            using var heldLock1 = locker.GetLock(lock1, 10);

            //assert
            await unlockTask;
            await secondPart;
            Assert.True(heldLock1.Obtained);
            Assert.True(lockTaskResult.Item1.Obtained);
            Assert.False(lockTaskResult.Item2.Obtained);

        }

    }
}
