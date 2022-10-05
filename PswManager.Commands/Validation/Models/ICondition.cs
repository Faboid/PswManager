namespace PswManager.Commands.Validation.Models;

/// <summary>
/// Represents a condition that can be validated.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICondition<T> {

    /// <summary>
    /// </summary>
    /// <returns>The error message built upon this condition's failure.</returns>
    string GetErrorMessage();

    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>Whether the condition is respected by the given <paramref name="obj"/>.</returns>
    bool IsValid(T obj);
}

