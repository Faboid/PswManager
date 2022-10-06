using PswManager.UI.Console.Inner.Interfaces;

namespace PswManager.UI.Console;

/// <summary>
/// Provides methods for the creation, deletion, editing, and retrieving of accounts.
/// </summary>
public interface IAccountsManager : IAccountDeleter, IAccountEditor, IAccountReader, IAccountCreator { }