using System;

namespace PswManager.Commands.Unused.Parsing.Attributes;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ParseableKeyAttribute : Attribute {

    public string Key { get; init; }

    public ParseableKeyAttribute(string key) {
        Key = key;
    }

}
