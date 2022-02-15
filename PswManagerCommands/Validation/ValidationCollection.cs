using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerCommands.Validation {

    /// <summary>
    /// Provides a list that represents whether conditions are represented and their respective error messages.
    /// </summary>
    public class ValidationCollection : IValidationCollection {

        readonly Dictionary<int, (bool, string)> validatorsDictionary = new();
        readonly string[] args;

        //several default string that gets used as error messages. They are being assigned this way mostly for unit testing purposes.
        public const string ArgumentsNullMessage = "The arguments' array cannot be null.";
        public const string ArgumentsNullOrEmptyMessage = "No value can be left empty.";
        public const string WrongArgumentsNumberMessage = "Incorrect arguments number.";
        public const string InvalidEmailMessage = "The provided email is invalid.";

        public int NullIndexCondition => -1;
        public int CorrectArgsNumberIndexCondition => -2;
        public int NullOrEmptyArgsIndexCondition => -3;

        public ValidationCollection(string[] arguments) {
            args = arguments;
        }

        /// <summary>
        /// Returns a <see cref="IReadOnlyList{(bool, string)}"/> that represents whether the conditions were respected, and their respective error messages.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<(bool condition, string errorMessage)> GetResult() {
            return validatorsDictionary.Select(x => x.Value).ToList();
        }

        /// <summary>
        /// Returns the stored arguments.
        /// </summary>
        /// <returns></returns>
        public string[] GetArguments() {
            return args;
        }

        /// <summary>
        /// Adds the values to the list.<br/>
        /// <paramref name="condition"/>:
        /// <br/>- true = validation succeeds
        /// <br/>- false = validation failure
        /// </summary>
        public void Add(ushort index, bool condition, string errorMessage) {
            validatorsDictionary.Add(index, (condition, errorMessage));
        }

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
        public void Add(IndexHelper index, Func<string[], bool> conditionFunction, string errorMessage) {
            if(!IndexesAreValid(index.RequiredSuccesses)) {
                return;
            }

            validatorsDictionary.Add(index.Index, (conditionFunction.Invoke(args), errorMessage));
        }

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
        public void AddCommonConditions(int minLength, int maxLength) {

            validatorsDictionary.Add(NullIndexCondition, (args != null, ArgumentsNullMessage));
            Add(new IndexHelper(CorrectArgsNumberIndexCondition, NullIndexCondition), (args) => args.Length >= minLength && args.Length <= maxLength, WrongArgumentsNumberMessage);
            Add(new IndexHelper(NullOrEmptyArgsIndexCondition, NullIndexCondition), (args) => args.All(x => string.IsNullOrWhiteSpace(x) == false), ArgumentsNullOrEmptyMessage);
        }

        /// <summary><inheritdoc/></summary>
        /// <param name="indexes"><inheritdoc/></param>
        public bool IndexesAreValid(params int[] indexes) {
            return indexes.All(x
                => validatorsDictionary.TryGetValue(x, out (bool valid, string _) result) && result.valid);
        }
    }


    internal class ValidationCollection<T> : IValidationCollection<T> {

        private readonly Dictionary<int, (bool, string)> validatorsDictionary = new();
        private readonly T obj;

        public ValidationCollection(T input) {
            obj = input;
        }

        public IReadOnlyList<(bool condition, string errorMessage)> GetResult() {
            return validatorsDictionary.Select(x => x.Value).ToList();
        }

        public T GetObject() {
            return obj;
        }

        public void Add(ushort index, bool condition, string errorMessage) {
            validatorsDictionary.Add(index, (condition, errorMessage));
        }

        public void Add(IndexHelper index, Func<T, bool> conditionFunction, string errorMessage) {
            if(!IndexesAreValid(index.RequiredSuccesses)) {
                return;
            }

            validatorsDictionary.Add(index.Index, (conditionFunction.Invoke(obj), errorMessage));
        }

        public bool IndexesAreValid(params int[] indexes) {
            return indexes.All(x
                => validatorsDictionary.TryGetValue(x, out (bool valid, string _) result) && result.valid);
        }

    }
}
