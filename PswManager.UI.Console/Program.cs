﻿using System;
using System.Threading;
using PswManager.Core.Services;
using System.IO.Abstractions;
using PswManager.Paths;
using PswManager.UI.Console;
using PswManager.Encryption.Services;

UserInput userInput = new();

string token = "A token to validate passwords.";

var fileSystem = new FileSystem();
var directoryInfoFactory = fileSystem.DirectoryInfo;
var fileInfoFactory = fileSystem.FileInfo;
var cryptoServiceFactory = new CryptoServiceFactory();
var keyGeneratorServiceFactory = new KeyGeneratorServiceFactory();
var pathsHandler = new PathsBuilder(directoryInfoFactory);
var tokenFactory = new TokenServiceFactory(pathsHandler, fileInfoFactory);
var tokenService = tokenFactory.CreateTokenService(token);
var cryptoFactory = new CryptoAccountServiceFactory(tokenService, cryptoServiceFactory, keyGeneratorServiceFactory);
var logInService = new LogInService(userInput, tokenService, cryptoFactory);
var cryptoAccount = await logInService.AskUserPasswordsAsync();

Console.WriteLine("Welcome to PswManager! Please insert a command.");
var cmdLoop = new CommandLoop(userInput, cryptoAccount);

await cmdLoop.StartAsync();

Console.WriteLine("Bye bye!");
Thread.Sleep(500);

