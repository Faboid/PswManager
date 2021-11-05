using System;
using System.Threading;
using PswManagerLibrary;
using PswManagerLibrary.Commands;
using PswManagerLibrary.Exceptions;

namespace PswManagerConsole {
    class Program {
        static void Main(string[] args) {

            Console.WriteLine("Welcome to PswManager! Please insert a command.");

            CommandQuery cq = new CommandQuery(new PswManagerLibrary.Global.Paths());

            //todo - a temporary password for testing. Will be removed once the initial setup is fully complete.
            string tempPassword = "psw gheerwiahgkth ewrgrthrer";
            Command cmmpsw = new Command(tempPassword);

            cq.Start(cmmpsw);

            string command;
            while((command = Console.ReadLine().ToLowerInvariant()) is not "exit") {

                try {
                    Command cmm = new Command(command);

                    var result = cq.Start(cmm);

                    Console.WriteLine(result);

                } catch(InvalidCommandException ex) {
                    Console.WriteLine($"Error: {ex.Message}");
                }

            }

            Console.WriteLine("Bye bye!");
            Thread.Sleep(500);
        }
    }
}
