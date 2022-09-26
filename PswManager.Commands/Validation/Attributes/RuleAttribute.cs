using System;

namespace PswManager.Commands.Validation.Attributes;
[AttributeUsage(AttributeTargets.Property)]
public abstract class RuleAttribute : Attribute {

    public RuleAttribute(string errorMessage) {
        ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
}
