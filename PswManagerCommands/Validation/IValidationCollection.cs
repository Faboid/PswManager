using System;
using System.Collections.Generic;

namespace PswManagerCommands.Validation {
    public interface IValidationCollection {

        /// <summary>
        /// Returns a <see cref="IReadOnlyList{(bool, string)}"/> that represents whether the conditions were respected, and their respective error messages.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<(bool condition, string errorMessage)> GetResult();

        /// <summary>
        /// Returns the stored arguments.
        /// </summary>
        /// <returns></returns>
        public string[] GetArguments();

        /// <summary>
        /// Adds the values to the list.<br/>
        /// <paramref name="condition"/>:
        /// <br/>- true = validation succeeds
        /// <br/>- false = validation failure
        /// </summary>
        void Add(bool condition, string errorMessage);

        /// <summary>
        /// Runs <paramref name="conditionFunction"/> within a trycatch and inserts the value as false in case an exception is thrown.<br/>
        /// Note:
        /// <br/>- true = validation succeeds
        /// <br/>- false = validation failure
        /// </summary>
        void Add(Func<string[], bool> conditionFunction, string errorMessage);

        /// <summary>
        /// Adds conditions to check whether the arguments are null, empty, or the wrong number.
        /// </summary>
        /// <param name="minLength">The minimum number of arguments there should be.</param>
        /// <param name="maxLength">The maximum number of arguments there should be.</param>
        void AddCommonConditions(int minLength, int maxLength);

    }
}
