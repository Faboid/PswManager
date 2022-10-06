using PswManager.Core.Services;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.Interfaces;
using PswManager.Database.Models;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using PswManager.UI.Console.Inner.Interfaces;

namespace PswManager.UI.Console.Inner;
public class AccountEditor : IAccountEditor {

    private readonly IDataEditor dataEditor;
    private readonly ICryptoAccountService cryptoAccount;

    public AccountEditor(IDataEditor dataEditor, ICryptoAccountService cryptoAccount) {
        this.dataEditor = dataEditor;
        this.cryptoAccount = cryptoAccount;
    }

    public EditorResponseCode UpdateAccount(string name, IReadOnlyAccountModel newValues) {
        return UpdateAccountAsync(name, newValues).GetAwaiter().GetResult();
    }

    public async Task<EditorResponseCode> UpdateAccountAsync(string name, IReadOnlyAccountModel newValues) {

        if(string.IsNullOrWhiteSpace(name)) {
            return EditorResponseCode.InvalidName;
        }

        var encryptedModel = await Task.Run(() => EncryptModel(newValues)).ConfigureAwait(false);
        return await dataEditor.UpdateAccountAsync(name, encryptedModel).ConfigureAwait(false);
    }

    [Pure]
    private IAccountModel EncryptModel(IReadOnlyAccountModel args) {
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
