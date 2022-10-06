using System;

namespace PswManager.Commands.Validation.Models;

/// <summary>
/// Represents a generic condition.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Condition<T> : ICondition<T> {

    /// <summary>
    /// Instantiates <see cref="Condition{T}"/>.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="logic"></param>
    /// <param name="errorMessage"></param>
    public Condition(IndexHelper index, Func<T, bool> logic, string errorMessage) {
        Index = index;
        Logic = logic;
        ErrorMessage = errorMessage;
    }

    private readonly string ErrorMessage;
    public IndexHelper Index { get; init; }
    public Func<T, bool> Logic { get; init; }

    public string GetErrorMessage() => ErrorMessage;

    public bool IsValid(T obj) {
        try {
            return Logic.Invoke(obj);
        } catch {
            return false;
        }
    }

}
