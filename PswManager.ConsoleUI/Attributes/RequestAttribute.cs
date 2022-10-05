using System;

namespace PswManager.ConsoleUI.Attributes;

/// <summary>
/// Requests <see cref="Requester"/> to ask for property to the console.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class RequestAttribute : Attribute {

    /// <summary>
    /// The name of the property.
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// The message used to request it to the user.
    /// </summary>
    public string RequestMessage { get; private set; }

    /// <summary>
    /// Whether the property can be left empty.
    /// </summary>
    public bool Optional { get; private set; }

    public RequestAttribute(string displayName, string requestMessage) {
        DisplayName = displayName;
        RequestMessage = requestMessage;
        Optional = false;
    }

    public RequestAttribute(string displayName, string requestMessage, bool optional) {
        DisplayName = displayName;
        RequestMessage = requestMessage;
        Optional = optional;
    }

    public RequestAttribute(string displayName, bool optional, params string[] multiLinedRequestMessage) {
        DisplayName = displayName;
        Optional = optional;
        RequestMessage = string.Join(Environment.NewLine, multiLinedRequestMessage);
    }

}
