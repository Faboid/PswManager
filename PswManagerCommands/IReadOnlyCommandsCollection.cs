namespace PswManagerCommands {

    public interface IReadOnlyCommandsCollection {

        ICommand this[string key] { get; }

    }
}
