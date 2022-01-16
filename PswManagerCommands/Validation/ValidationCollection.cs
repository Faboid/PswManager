﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerCommands.Validation {

    /// <summary>
    /// Provides a list that represents whether conditions are represented and their respective error messages.
    /// </summary>
    public class ValidationCollection : IValidationCollection {

        readonly Dictionary<int, (bool, string)> validatorsDictionary = new();
        readonly string[] args;

        public const int NullIndexCondition = -1;
        public const int CorrectArgsNumberIndexCondition = -2;
        public const int NullOrEmptyArgsIndexCondition = -3;

        //several default string that gets used as error messages. They are being assigned this way mostly for unit testing purposes.
        public const string ArgumentsNullMessage = "The arguments' array cannot be null.";
        public const string ArgumentsNullOrEmptyMessage = "No value can be left empty.";
        public const string WrongArgumentsNumberMessage = "Incorrect arguments number.";
        public const string InvalidEmailMessage = "The provided email is invalid.";

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
        /// Runs <paramref name="conditionFunction"/> within a trycatch and inserts the value as false in case an exception is thrown.<br/>
        /// Note:
        /// <br/>- true = validation succeeds
        /// <br/>- false = validation failure
        /// </summary>
        public void Add(ushort index, Func<string[], bool> conditionFunction, string errorMessage) {

            Add((int)index, conditionFunction, errorMessage);
        }

        private void Add(int index, Func<string[], bool> conditionFunction, string errorMessage) {
            try {
                validatorsDictionary.Add(index, (conditionFunction.Invoke(args), errorMessage));
            } catch(Exception) {
                validatorsDictionary.Add(index, (false, errorMessage));
            }
        }

        /// <summary>
        /// Adds conditions to check whether the arguments are null, empty, or the wrong number.
        /// </summary>
        /// <param name="minLength">The minimum number of arguments there should be.</param>
        /// <param name="maxLength">The maximum number of arguments there should be.</param>
        public void AddCommonConditions(int minLength, int maxLength) {

            validatorsDictionary.Add(NullIndexCondition, (args != null, ArgumentsNullMessage));
            Add(CorrectArgsNumberIndexCondition, (args) => args.Length >= minLength && args.Length <= maxLength, WrongArgumentsNumberMessage);
            Add(NullOrEmptyArgsIndexCondition, (args) => args.All(x => string.IsNullOrWhiteSpace(x) == false), ArgumentsNullOrEmptyMessage);
        }

    }
}
