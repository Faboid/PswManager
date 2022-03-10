using System.Collections.Generic;

namespace PswManagerCommands.Validation {
    public interface ICondition<T> {
        string GetErrorMessage();
        bool IsValid(T obj);
        bool IsValid(T obj, IList<int> failedConditions);
    }

}
