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

            //act
            bool correctIsInUse = refCount.UseValue(x => refCount.IsInUse);
            var task = refCount.UseValueAsync(async () => {
                orderChecker.Done(1);
                await orderChecker.WaitFor(2, 200);
            });
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
