using System;
using System.Threading;
using PswManagerLibrary.UIConnection;
using PswManagerLibrary.Cryptography;
using PswManagerConsole;

UserInput userInput = new();

var cryptoFactory = new CryptoFactory(userInput);
var cryptoAccount = await cryptoFactory.AskUserPasswordsAsync();

Console.WriteLine("Welcome to PswManager! Please insert a command.");
var cmdLoop = new CommandLoop(userInput, cryptoAccount);

await cmdLoop.StartAsync();

Console.WriteLine("Bye bye!");
Thread.Sleep(500);

