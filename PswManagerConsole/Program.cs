using System;
using System.Threading;
using PswManagerLibrary.UIConnection;
using PswManagerLibrary.Cryptography;
using PswManagerConsole;

UserInput userInput = new();

var cryptoAccount = new CryptoFactory(userInput).AskUserPasswords();

Console.WriteLine("Welcome to PswManager! Please insert a command.");

var cmdLoop = new CommandLoop(userInput, cryptoAccount);

cmdLoop.Start();

Console.WriteLine("Bye bye!");
Thread.Sleep(500);

