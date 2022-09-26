using PswManager.ConsoleUI.Inner.Interfaces;

namespace PswManager.ConsoleUI;
public interface IAccountsManager : IAccountDeleter, IAccountEditor, IAccountReader, IAccountCreator { }