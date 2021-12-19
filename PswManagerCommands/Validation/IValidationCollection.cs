using PswManagerLibrary.Storage;
using System;
using System.Collections.Generic;

namespace PswManagerCommands.Validation {
    public interface IValidationCollection {

        public IReadOnlyList<(bool condition, string errorMessage)> GetResult();
        public string[] GetArguments();
        void Add(bool condition, string errorMessage);
        void Add(Func<string[], bool> conditionFunction, string errorMessage);
        void AddCommonConditions(int minLength, int maxLength);
        void AddAccountShouldExistCondition(IPasswordManager pswManager);

    }
}
