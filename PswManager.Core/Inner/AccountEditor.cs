using PswManager.Core.Cryptography;
using PswManager.Core.Inner.Interfaces;
using PswManager.Database.DataAccess.ErrorCodes;
using PswManager.Database.DataAccess.Interfaces;
using PswManager.Database.Models;
using PswManager.Utils;
using System.Threading.Tasks;

namespace PswManager.Core.Inner {
    public class AccountEditor : IAccountEditor {

        private readonly IDataEditor dataEditor;
        private readonly ICryptoAccount cryptoAccount;

        public AccountEditor(IDataEditor dataEditor, ICryptoAccount cryptoAccount) {
            this.dataEditor = dataEditor;
            this.cryptoAccount = cryptoAccount;
        }

        public Option<EditorErrorCode> UpdateAccount(string name, AccountModel newValues) {

            if(string.IsNullOrWhiteSpace(name)) {
                return EditorErrorCode.InvalidName;
            }

            EncryptModel(newValues);
            return dataEditor.UpdateAccount(name, newValues);
        }

        public async Task<Option<EditorErrorCode>> UpdateAccountAsync(string name, AccountModel newValues) {

            if(string.IsNullOrWhiteSpace(name)) {
                return EditorErrorCode.InvalidName;
            }

            await Task.Run(() => EncryptModel(newValues)).ConfigureAwait(false);
            return await dataEditor.UpdateAccountAsync(name, newValues).ConfigureAwait(false);
        }

        private void EncryptModel(AccountModel args) {
            if(!string.IsNullOrWhiteSpace(args.Password)) {
                args.Password = cryptoAccount.GetPassCryptoService().Encrypt(args.Password);
            }
            if(!string.IsNullOrWhiteSpace(args.Email)) {
                args.Email = cryptoAccount.GetEmaCryptoService().Encrypt(args.Email);
            }
        }

    }
}
