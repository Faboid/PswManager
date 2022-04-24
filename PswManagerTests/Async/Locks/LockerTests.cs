using PswManagerAsync.Locks;
using PswManagerTests.Async.TestsHelpers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Async.Locks {
    public class LockerTests {

        [Fact]
        public async Task LocksAndUnlocks() {

            //arrange
            Locker locker = new();
            OrderChecker orderChecker = new();
            bool firstLockResult;
            bool failLockResult;
            bool secondLockResult;
            bool thirdLockResult;

            //act
            orderChecker.Done(1);
            
            using(var lock1 = locker.GetLock()) {
                firstLockResult = lock1.Obtained;

                using(var lockfail = await locker.GetLockAsync(5)) {
                    failLockResult = lockfail.Obtained;
                    if(lockfail.Obtained) {
                        OrderChecker.Never();
                    }
                }

                orderChecker.Done(2);
            }
            using(var lock2 = await locker.GetLockAsync(100, CancellationToken.None).ThrowIfTakesOver(50)) {
                secondLockResult = lock2.Obtained;
                orderChecker.Done(3);
            }
            using var lock3 = await locker.GetLockAsync(10).ThrowIfTakesOver(50);
            thirdLockResult = lock3.Obtained;
            orderChecker.Done(4);

            //assert
            Assert.True(firstLockResult);
            Assert.True(secondLockResult);
            Assert.True(thirdLockResult);
            Assert.False(failLockResult);

        }

        [Fact]
        public async Task LocksInDifferentThreads() {

            //arrange
            Locker locker = new();
            OrderChecker checker = new();
            TaskFactory factory = new();

            //act & assert
            Task task1 = factory.StartNew(async () => {
                using var lock1 = await locker.GetLockAsync(10);
                await checker.WaitFor(2, 100);
                checker.Done(3);
            });
            Task task2 = factory.StartNew(async () => {
                await checker.WaitFor(1, 20);
                checker.Done(2);
                using var lock2 = await locker.GetLockAsync(20);
                checker.Done(4);
            });

            checker.Done(1);
            await task2;
            await task1;

        }

        [Fact]
        public async Task ThrowsWhenCanceled() {

            //arrange
            Locker locker = new();
            CancellationTokenSource cts = new();
            cts.Cancel();
            
            //assert
            Assert.Throws<OperationCanceledException>(() => locker.GetLock(10, cts.Token));
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await locker.GetLockAsync(10, cts.Token));

        }

    }
}
