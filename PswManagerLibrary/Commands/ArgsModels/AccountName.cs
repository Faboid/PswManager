using PswManagerCommands;
using PswManagerLibrary.UIConnection.Attributes;

namespace PswManagerLibrary.Commands.ArgsModels {
    public class AccountName : ICommandInput {

        [Request("Name", "Insert the name of the account you wish to delete.")]
        public string Name { get; set; }

    }
}
