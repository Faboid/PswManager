# PswManager

PswManager is a password manager application developed using WPF and console. It provides a secure way to store and manage passwords locally.

Please note that this application is created for learning purposes; I am not responsible for any damage caused by its usage. While it is well-tested with XUnit, the lack of QA might lead to unforeseen bugs that lead to the loss of stored data. For that reason, it's highly discouraged to use this application for anything important.

## Features

- Password Storage: PswManager allows you to securely store passwords as "accounts". Each account consists of a Name, Password, and Email. The Name field serves as a primary key and is stored in an unencrypted form. The Password and Email fields are encrypted using two passwords generated from a master key.
- Authentication: The application uses a token file to authenticate the master key, ensuring secure access to the stored passwords.
- Multiple Database Support: PswManager supports four types of databases: SQLite, JSON, Text, and an in-memory database for testing purposes. Please note that the database type needs to be manually changed in the code.
- User Interface Options: PswManager provides two interface options: WPF and console. To choose the interface, set up the respective csproj file as the startup project in your development environment.

## Installation and Usage

1. Clone or download the repository.
2. Open the project in Visual Studio.
3. Build the solution.
4. Set up the desired csproj file (WPF or console) as the startup project in your development environment.
5. Choose which database to use. Currently, supported databases are: SQLite, JSON, and Text files. In-memory is supported as well, but it doesn't persist throughout sessions.
   1. If you're building it from the console project:
      - Locate the `CommandLoop` class in the code.
      - Modify the enum used in its constructor to set the desired database type (`SQLite`, `Json`, `TextFile`, or `InMemory`).
   2. If you're building it from the WPF project:
      - Locate the `App.xaml.cs` file in the code.
      - Modify the enum used for `.AddAccountsPipeline()` to set the desired database type (`SQLite`, `Json`, `TextFile`, or `InMemory`).
6. Run the application using the chosen user interface (WPF or console).
7. If you're using the console, use the command "help" to find a list of commands.
