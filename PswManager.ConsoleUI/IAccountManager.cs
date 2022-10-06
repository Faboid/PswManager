using PswManager.ConsoleUI.Inner.Interfaces;

namespace PswManager.ConsoleUI;

/// <summary>
/// Provides methods for the creation, deletion, editing, and retrieving of accounts.
/// </summary>
public interface IAccountsManager : IAccountDeleter, IAccountEditor, IAccountReader, IAccountCreator { }