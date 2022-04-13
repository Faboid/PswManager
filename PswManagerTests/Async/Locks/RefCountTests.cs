using PswManagerAsync.Locks;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Async.Locks {
    public class RefCountTests {

        [Fact]
        public async Task IsInUse_Correct() {

            //arrange
            RefCount<int> refCount = new(5);

            //act
            bool correctIsInUse = refCount.UseValue(x => refCount.IsInUse);
            var task = refCount.UseValueAsync(async () => await Task.Delay(50));
            bool asyncIsInUse = refCount.IsInUse;
            await task;
            bool afterAsyncIsInUse = refCount.IsInUse;
            
            //assert
            Assert.True(correctIsInUse);
            Assert.True(asyncIsInUse);
            Assert.False(afterAsyncIsInUse);

        }

    }
}
