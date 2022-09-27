using PswManager.ConsoleUI.Inner.Interfaces;
using PswManager.Core.Services;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using PswManager.Database.Models;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace PswManager.ConsoleUI.Inner;
public class AccountEditor : IAccountEditor {

    private readonly IDataEditor dataEditor;
    private readonly ICryptoAccountService cryptoAccount;

    public AccountEditor(IDataEditor dataEditor, ICryptoAccountService cryptoAccount) {
        this.dataEditor = dataEditor;
        this.cryptoAccount = cryptoAccount;
    }

    public EditorResponseCode UpdateAccount(string name, AccountModel newValues) {
        return UpdateAccountAsync(name, newValues).GetAwaiter().GetResult();
    }

    public async Task<EditorResponseCode> UpdateAccountAsync(string name, AccountModel newValues) {

        if(string.IsNullOrWhiteSpace(name)) {
            return EditorResponseCode.InvalidName;
        }

        var encryptedModel = await Task.Run(() => EncryptModel(newValues)).ConfigureAwait(false);
        return await dataEditor.UpdateAccountAsync(name, encryptedModel).ConfigureAwait(false);
    }

    [Pure]
    private AccountModel EncryptModel(AccountModel args) {
        var output = new AccountModel(args.Name, args.Password, args.Email);
        if(!string.IsNullOrWhiteSpace(output.Password)) {
            output.Password = cryptoAccount.GetPassCryptoService().Encrypt(output.Password);
        }
        if(!string.IsNullOrWhiteSpace(output.Email)) {
            output.Email = cryptoAccount.GetEmaCryptoService().Encrypt(output.Email);
        }
        return output;
    }

}
