using PswManagerLibrary.Storage;

namespace PswManagerCommands {
    public interface ICommandsCollection {

        ICommand this[string key] { get; }
        void Add(string key, ICommand command);
        void AddDefault(IPasswordManager passwordManager);
        IReadOnlyCommandsCollection AsReadOnly();

    }
}