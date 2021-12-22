using System.Linq;

namespace PswManagerLibrary.OldCommands {

    /// <summary>
    /// A class that contains a main command and its arguments.
    /// </summary>
    public class Command {

        public Command(string fullCommand) {
            string[] all = fullCommand.Split(' ');

            for(int i = 0; i < all.Length; i++) {
                all[i] = all[i].Trim();
            }

            MainCommand = CommandTypeAnalyzer.Get(all[0].ToLowerInvariant());

            if(all.Length > 1) {
                this.Arguments = all.Skip(1).ToArray();
            }
        }

        public CommandType MainCommand { get; set; }
        public string[] Arguments { get; set; }

    }
}
