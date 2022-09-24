namespace PswManager.Core.AccountModels;

public interface IAccountModelFactory {
    IAccountModel CreateDecryptedAccount(string name, string password, string email);
    IAccountModel CreateEncryptedAccount(string name, string password, string email);
}