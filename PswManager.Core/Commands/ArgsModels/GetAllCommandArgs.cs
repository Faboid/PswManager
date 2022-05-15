using PswManager.Commands;
using PswManager.Core.Commands.Validation.Attributes;
using PswManager.Core.UIConnection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManager.Core.Commands.ArgsModels {
    public class GetAllCommandArgs : ICommandInput {

        [NoDuplicateValues(GetAllCommand.DuplicateKeyErrorMessage)]
        [ValidValues(GetAllCommand.InexistentKeyErrorMessage, "names", "passwords", "emails")]
        [Request("Keys", true, "Leave empty if you want all values.",
            "If you only want specific ones, insert their keys: names, passwords, emails.",
            "Properly put a single space between the keys, if you require multiple.")]
        public string Keys { get; private set; }

        public IEnumerable<string> SplitKeys() => Keys.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x));

    }
}
