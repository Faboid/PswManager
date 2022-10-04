using System;
using System.Reflection;

namespace PswManager.Commands.Validation.Attributes;
[AttributeUsage(AttributeTargets.Property)]
public class RequiredAttribute : Attribute {

    public string GetErrorMessage(PropertyInfo prop) {
        return ErrorMessage ?? BuildErrorMessage(prop.Name);
    }

    private readonly string ErrorMessage;

    public RequiredAttribute() { }

    public RequiredAttribute(string displayName) : base() {
        ErrorMessage = BuildErrorMessage(displayName);
    }

    private static string BuildErrorMessage(string displayName) => $"You must provide a value for the {displayName}.";

}
