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
                using var locktwo = await locker.GetLockAsync(lock2);
                orderChecker.Done(4);
            }

            async Task UnlockLogic(NamesLocker.Lock heldLock2) {
                await orderChecker.WaitForAsync(2, 100);
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

        [Fact]
        public async Task SuccessfullyLockAll() {

            //arrange
            NamesLocker locker = new();
            string name = "name";

            //act
            var namedLock = await locker.GetLockAsync(name, 10);
            var nestedMainLock = await locker.GetAllLocksAsync(10);
            nestedMainLock.Dispose();
            namedLock.Dispose();

            var mainLock = locker.GetAllLocks(50);
            var nestedNamedLock = await locker.GetLockAsync(name, 10);
            nestedNamedLock.Dispose();
            mainLock.Dispose();

            using var freedLock = await locker.GetLockAsync(name, 10);

            //assert
            Assert.True(namedLock.Obtained);
            Assert.False(nestedMainLock.Obtained);
            Assert.True(mainLock.Obtained);
            Assert.False(nestedNamedLock.Obtained);
            Assert.True(freedLock.Obtained);

        }

        [Fact]
        public async Task LockAllFreesCorrectly() {

            //arrange
            NamesLocker locker = new();
            string name = "name";

            //act
            var namedLock = locker.GetLock(name, 10);
            var allLocksTask = locker.GetAllLocksAsync(100);
            namedLock.Dispose();
            var allLocks = await allLocksTask;

            var failLock = await locker.GetLockAsync("someNewLock", 5);
            failLock.Dispose();

            var newNamedLockTask = locker.GetLockAsync(name, 100);
            allLocks.Dispose();
            var newNamedLock = await newNamedLockTask;
            newNamedLock.Dispose();

            //assert
            Assert.True(namedLock.Obtained);
            Assert.True(allLocks.Obtained);
            Assert.False(failLock.Obtained);
            Assert.True(newNamedLock.Obtained);

        }

    }
}
