using PswManagerAsync;
using PswManagerTests.Async.TestsHelpers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PswManagerTests.Async {
    public class AsyncLazyTests {

        [Fact]
        public async Task InitializeLazilyAsync() {

            //arrange
            AsyncLazy<ExpensiveClass> lazy;
            OrderChecker orderChecker = new();

            //act
            lazy = ExpensiveClass.CreateAsync(orderChecker);
            await orderChecker.WaitForAsync(1, 50);
            bool shouldNotBeReady = lazy.IsValueCreated;
            orderChecker.Done(2);

            //wait for it
            Task<ExpensiveClass> implicitConversionTask = lazy;
            var cached = await implicitConversionTask.ConfigureAwait(false);

            //assert
            Assert.False(shouldNotBeReady);
            Assert.Same(cached, await lazy.Value.ConfigureAwait(false));
            Assert.True(lazy.IsValueCreated);
            Assert.True(cached.IsInitialized);

        }

        [Fact]
        public void InitializeLazilySync() {

            //arrange
            AsyncLazy<ExpensiveClass> lazy;
            OrderChecker orderChecker = new();

            //act
            lazy = new(() => ExpensiveClass.Create(orderChecker));
            orderChecker.WaitFor(1, 50);
            bool shouldNotBeReady = lazy.IsValueCreated;
            orderChecker.Done(2);

            var cached = lazy.Value.Result;

            //assert
            Assert.False(shouldNotBeReady);
            Assert.Same(cached, lazy.Value.Result);
            Assert.True(lazy.IsValueCreated);
            Assert.True(cached.IsInitialized);

        }

    }

    internal class ExpensiveClass {

        private ExpensiveClass() {
            IsInitialized = true;
        }

        public bool IsInitialized = false;

        public static ExpensiveClass Create(OrderChecker orderChecker) {
            orderChecker.Done(1);
            orderChecker.WaitFor(2, 50);
            Thread.Sleep(20);
            orderChecker.Done(3);
            return new ExpensiveClass();
        }

        public static async Task<ExpensiveClass> CreateAsync(OrderChecker orderChecker) {
            orderChecker.Done(1);
            await orderChecker.WaitForAsync(2, 50);
            await Task.Delay(20);
            orderChecker.Done(3);
            return new ExpensiveClass();
        }

    }

}
