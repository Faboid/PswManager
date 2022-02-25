using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManagerCommands.Validation {

    public class ValidationCollection<T> : IValidationCollection<T> {

        private readonly Dictionary<int, (bool, string)> validatorsDictionary = new();
        private readonly T obj;
        //todo implement use of auto-validation's class

        public ValidationCollection(T input) {
            obj = input;
        }

        public IReadOnlyList<(bool condition, string errorMessage)> GetResult() {
            return validatorsDictionary.Select(x => x.Value).ToList();
        }

        //todo - consider adding an inplicit conversion to simplify operations that require the object
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
