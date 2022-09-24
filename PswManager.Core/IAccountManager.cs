using PswManager.Core.Inner.Interfaces;
namespace PswManager.Core;
public interface IAccountsManager : IAccountDeleter, IAccountEditor, IAccountReader, IAccountCreator { }