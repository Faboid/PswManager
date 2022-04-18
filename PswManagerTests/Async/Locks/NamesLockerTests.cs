using PswManagerAsync.Locks;
using PswManagerTests.Async.TestsHelpers;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Async.Locks {
    public class NamesLockerTests {

        [Fact]
        public async Task DifferentiatesDifferentNames() {

            //arrange
            using NamesLocker locker = new();
            string lock1 = "Hello!";
            string lock2 = "Sup!";
            using OrderChecker orderChecker = new();

            //act
            async Task<LockResult> LockLogic() {
                orderChecker.Done(1);
                locker.Lock(lock1);
                await locker.LockAsync(lock2);
                var result = await locker.LockAsync(lock1, 10);
                orderChecker.Done(2);
                await locker.LockAsync(lock2);
                orderChecker.Done(4);
                return result;
            }

            async Task UnlockLogic() {
                await orderChecker.WaitFor(2, 100);
                locker.Unlock(lock2);
                orderChecker.Done(3);
            }

            var lockTask = LockLogic().ThrowIfTakesOver(1000);
            var unlockTask = UnlockLogic().ThrowIfTakesOver(1000);

            //assert
            var result = await lockTask;
            await unlockTask;
            Assert.True(result.Failed);

        }

    }
}
