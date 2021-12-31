using System;
using System.Threading;
using PswManagerLibrary.UIConnection;
using PswManagerLibrary.Storage;
using PswManagerLibrary.Global;
using PswManagerLibrary.Cryptography;

namespace PswManagerConsole {
    class Program {
        static void Main(string[] args) {

            Paths paths = new Paths();
            UserInput userInput = new UserInput();
            //a temporary password for testing. Will be removed once the initial setup is fully complete.
            CryptoAccount cryptoAccount = new CryptoAccount("gheerwiahgkth", "ewrgrthrer");

            Console.WriteLine("Welcome to PswManager! Please insert a command.");

            CommandLoop cmdLoop = new CommandLoop(userInput, new PasswordManager(paths, cryptoAccount));

            cmdLoop.Start();

            //string command;
            //while((command = Console.ReadLine().ToLowerInvariant()) is not "exit") {

            //    try {
            //        Command cmm = new Command(command);

            //        var result = cq.Start(cmm);

            //        Console.WriteLine(result);

            //    } catch(InvalidCommandException ex) {
            //        Console.WriteLine($"Error: {ex.Message}");
            //    }

            //}

            Console.WriteLine("Bye bye!");
            Thread.Sleep(500);
        }
    }
}
