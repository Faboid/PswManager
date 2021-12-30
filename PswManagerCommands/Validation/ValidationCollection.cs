using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PswManagerCommands.Validation {

    /// <summary>
    /// Provides a list that represents whether conditions are represented and their respective error messages.
    /// </summary>
    public class ValidationCollection : IValidationCollection {

        readonly List<(bool, string)> validators = new();
        readonly string[] args;

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
            return validators.AsReadOnly();
        }

        /// <summary>
        /// Returns the stored arguments.
        /// </summary>
        /// <returns></returns>
        public string[] GetArguments() {
            return args;
        }

        /// <summary>
        /// Adds the values to the list.
        /// </summary>
        public void Add(bool condition, string errorMessage) {
            validators.Add((condition, errorMessage));
        }

        /// <summary>
        /// Runs <paramref name="conditionFunction"/> within a trycatch and inserts the value as false in case an exception is thrown.
        /// </summary>
        public void Add(Func<string[], bool> conditionFunction, string errorMessage) {
            try {
                Add(conditionFunction.Invoke(args), errorMessage);
            } catch(Exception) {
                Add(false, errorMessage);
            }
        }

        /// <summary>
        /// Adds conditions to check whether the arguments are null, empty, or the wrong number.
        /// </summary>
        /// <param name="minLength">The minimum number of arguments there should be.</param>
        /// <param name="maxLength">The maximum number of arguments there should be.</param>
        public void AddCommonConditions(int minLength, int maxLength) {
            Add(args != null, ArgumentsNullMessage);
            Add((args) => args.Length >= minLength && args.Length <= maxLength, WrongArgumentsNumberMessage);
            Add((args) => args.All(x => string.IsNullOrWhiteSpace(x) == false), ArgumentsNullOrEmptyMessage);
        }

        /// <summary>
        /// [WIP] - Adds a condition to check whether the provided email is valid.
        /// </summary>
        /// <param name="stringToCheck">Since certain commands do not require to insert the email in a specific index, it's possible to pass in the string to check directly.</param>
        public void AddEmailCheck(string stringToCheck) {
            throw new NotImplementedException();
            //todo - implement regex
            Regex regex = new Regex("");

            Add(regex.IsMatch(stringToCheck), InvalidEmailMessage);
        }
    }
}
