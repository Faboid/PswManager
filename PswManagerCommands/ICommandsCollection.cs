using System.Collections.Generic;

namespace PswManagerCommands {
    public interface ICommandsCollection {

        ICommand this[string key] { get; }
        void Add(string key, ICommand command);
        IReadOnlyDictionary<string, ICommand> AsReadOnly();

    }
}