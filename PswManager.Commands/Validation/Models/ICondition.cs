using System.Collections.Generic;

namespace PswManager.Commands.Validation.Models; 
public interface ICondition<T> {
    string GetErrorMessage();
    bool IsValid(T obj);
    bool IsValid(T obj, IList<int> failedConditions);
}

