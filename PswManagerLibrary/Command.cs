using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary {

    public class Command {

        public Command(string fullCommand) {
            string[] all = fullCommand.Split(' ');
            string command = all[0];
            this.arguments = all.Skip(1).ToArray();

            
        }

        
        public CommandType command { get; set; }
        public string[] arguments { get; set; }

    }

    public enum CommandType {

    }
}
