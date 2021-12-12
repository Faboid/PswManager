using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.RefactoringFolder.Commands.Validation {

    /// <summary>
    /// Provides a list that represents whether conditions are represented and their respective error messages.
    /// </summary>
    public class ValidationCollection {

        readonly List<(bool, string)> validators = new List<(bool, string)>();
        readonly string[] args;

        public ValidationCollection(string[] arguments) {
            args = arguments;
        }

        /// <summary>
        /// Returns a <see cref="IReadOnlyList{(bool, string)}"/> that represents whether the conditions were respected, and their respective error messages.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<(bool, string)> Get() {
            return validators.AsReadOnly();
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
            } catch (Exception) {
                Add(false, errorMessage);
            }
        }

    }
}
