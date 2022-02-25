using System;
using System.Collections.Generic;

namespace PswManagerCommands.Validation {

    public interface IValidationCollection<T> {

        /// <summary>
        /// Returns a <see cref="IReadOnlyList{(bool, string)}"/> that represents whether the conditions were respected, and their respective error messages.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<(bool condition, string errorMessage)> GetResult();

        /// <summary>
        /// Returns the stored arguments.
        /// </summary>
        /// <returns></returns>
        public T GetObject();

        /// <summary>
        /// Adds the values to the list.<br/>
        /// <paramref name="condition"/>:
        /// <br/>- true = validation succeeds
        /// <br/>- false = validation failure
        /// </summary>
        void Add(ushort index, bool condition, string errorMessage);

        /// <summary>
        /// Runs <paramref name="conditionFunction"/> within a trycatch and inserts the value as false in case an exception is thrown.
        /// <br/>Note:
        /// <br/>- true = validation succeeds
        /// <br/>- false = validation failure
        /// </summary>
        /// <param name="index">
        /// Uses <see cref="IndexHelper.Index"/> to assign the index and checks 
        /// <see cref="IndexHelper.RequiredSuccesses"/> to see whether this condition requires an already-assigned condition.
        /// <br/>If one of the required conditions is false or missing, the operation is voided and the condition won't be added.
        /// </param>
        void Add(IndexHelper index, Func<T, bool> conditionFunction, string errorMessage);

        /// <summary>
        /// Returns <see langword="True"/> if the given indexes exist and are valid. Otherwise; <see langword="False"/>.
        /// </summary>
        /// <param name="indexes">The indexes of the conditions to check.</param>
        bool IndexesAreValid(params int[] indexes);

    }
}
