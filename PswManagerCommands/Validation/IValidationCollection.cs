using System;
using System.Collections.Generic;

namespace PswManagerCommands.Validation {
    //todo - improve (and fix) documentation of both IValidationCollection and ValidationCollection.
    public interface IValidationCollection {

        public int NullIndexCondition { get; }
        public int CorrectArgsNumberIndexCondition { get; }
        public int NullOrEmptyArgsIndexCondition { get; }

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
        void Add(IndexHelper index, Func<string[], bool> conditionFunction, string errorMessage);

        /// <summary>
        /// Adds conditions to check whether the arguments are null, empty, or the wrong number.
        /// <br/>The used indexes are: 
        /// <br/>- <see cref="NullIndexCondition"/>
        /// <br/>- <see cref="CorrectArgsNumberIndexCondition"/>
        /// <br/>- <see cref="NullOrEmptyArgsIndexCondition"/>
        /// <br/><br/>The latter two conditions are only applied if the first is valid.
        /// </summary>
        /// <param name="minLength">The minimum number of arguments there should be.</param>
        /// <param name="maxLength">The maximum number of arguments there should be.</param>
        void AddCommonConditions(int minLength, int maxLength);

        /// <summary>
        /// Returns <see langword="True"/> if the given indexes exist and are valid. Otherwise; <see langword="False"/>.
        /// </summary>
        /// <param name="indexes">The indexes of the conditions to check.</param>
        bool IndexesAreValid(params int[] indexes);

    }
}
