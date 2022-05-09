using PswManagerAsync.Locks;
using PswManagerTests.Async.TestsHelpers;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Async.Locks {
    public class RefCountTests {

        [Fact]
        public async Task IsInUse_Correct() {

            //arrange
            RefCount<int> refCount = new(5);
            OrderChecker orderChecker = new();
            TaskFactory taskFactory = new();

            //act
            bool correctIsInUse;
            using(var reference = refCount.GetRef()) {
                correctIsInUse = refCount.IsInUse;
            }
            var task = await taskFactory.StartNew(async () => {
                using var reference = refCount.GetRef();
                orderChecker.Done(1);
                await orderChecker.WaitForAsync(2, 200);
            });
            await orderChecker.WaitForAsync(1, 100);
            bool asyncIsInUse = refCount.IsInUse;
            orderChecker.Done(2);
            await task;
            bool afterAsyncIsInUse = refCount.IsInUse;

            //assert
            Assert.True(correctIsInUse);
            Assert.True(asyncIsInUse);
            Assert.False(afterAsyncIsInUse);

        }

    }
}
