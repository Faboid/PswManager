using System;
using System.Threading.Tasks;

namespace PswManagerTests.Async.TestsHelpers {
    internal class OrderChecker {

        public int CurrentOperation { get; private set; } = 0;

        /// <summary>
        /// Signals that the operation number <paramref name="operationNum"/> has been completed.
        /// <br/>This method will make sure that the given <paramref name="operationNum"/> is the one following the previous operation number.
        /// If it's not, this method will throw <see cref="OrderException"/>.
        /// </summary>
        /// <param name="operationNum"></param>
        /// <exception cref="OrderException"></exception>
        public void Done(int operationNum) {
            CurrentOperation++;
            if(CurrentOperation != operationNum) {
                throw new OrderException($"The operation number {operationNum} has occurred instead of the number {CurrentOperation}");
            }
        }

        /// <summary>
        /// Uses <see cref="Task.Delay(int)"/> to wait asynchronously until <see cref="CurrentOperation"/> is equal or above of <paramref name="operationToWaitFor"/>.
        /// <br/><br/>
        /// If it takes more milliseconds than <paramref name="millisecondsTimeout"/>, it throws a <see cref="TimeoutException"/>.
        /// </summary>
        /// <param name="operationToWaitFor"></param>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public async Task WaitFor(int operationToWaitFor, int millisecondsTimeout) {

            int awaited = 0;
            int cycle = 10;
            while(CurrentOperation < operationToWaitFor) {
                if(awaited > millisecondsTimeout) {
                    throw new TimeoutException($"The operation number {operationToWaitFor} hasn't been reached. Current operation: {CurrentOperation}");
                }
                await Task.Delay(cycle);
                awaited += cycle;
            }

        }


    }

    internal class OrderException : Exception {

        public OrderException(string message) : base(message) {

        }
    
    }

}
