using PswManager.Commands;
using PswManager.ConsoleUI.Attributes;
using PswManager.ConsoleUI.Commands.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PswManager.ConsoleUI.Commands.ArgsModels;

/// <summary>
/// Represents the arguments given to get all accounts.
/// </summary>
public class GetAllCommandArgs : ICommandInput {

    /// <summary>
    /// The values to be taken from the account to be displayed. Leave empty to get all.
    /// </summary>
    [NoDuplicateValues(GetAllCommand.DuplicateKeyErrorMessage)]
    [ValidValues(GetAllCommand.InexistentKeyErrorMessage, "names", "passwords", "emails")]
    [Request("Keys", true, "Leave empty if you want all values.",
        "If you only want specific ones, insert their keys: names, passwords, emails.",
        "Properly put a single space between the keys, if you require multiple.")]
    public string Keys { get; private set; }

    public IEnumerable<string> SplitKeys() => Keys?.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)) ?? Array.Empty<string>();

}
